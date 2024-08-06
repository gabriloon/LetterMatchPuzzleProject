using DG.Tweening;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;
    private Vector3 minBounds;
    private Vector3 maxBounds;
    private GameObject boxPrefab;
    private GameObject jewelPrefab;
    private List<Vector3> usedPositions = new List<Vector3>();
    private Vector2 jewelBoardInitPosition = new Vector2(-8.1f, 2.5f);
    private float jewelBoardXPositionDiff = 1.8f;
    public PlayerView playerView;

    private List<JewelView> collectedJewelViews = new List<JewelView>();
    private Stack<JewelView> allJewelView = new Stack<JewelView>();
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject endButton;

    [SerializeField] private GameObject playerObject;
    [SerializeField] private Transform objectInParent;
    [SerializeField] private int initFillJewelBoardCount;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Screen.SetResolution(2400, 1080, FullScreenMode.Windowed);
        playButton.SetActive(true);
    }

    private void LoadResources()
    {
        boxPrefab = ResourceUtil.GetBoxObject();
        jewelPrefab = ResourceUtil.GetJewelObject();
    }

    public void Initialize()
    {
        LoadResources();
        collectedJewelViews = new List<JewelView>();
        allJewelView = new Stack<JewelView> { };
        CalculateBounds();
        CreatePlayerObject();
        CreateBoxObjects();
        InitializeJewelBoard(initFillJewelBoardCount);
        playerView.SetControllable(true);
        playButton.gameObject.SetActive(false);
        endButton.SetActive(false);
    }

    private void CreatePlayerObject()
    {
        if (playerObject == null) playerObject = Instantiate(ResourceUtil.GetPlayerObject(), Vector3.zero, Quaternion.identity);
        playerView = playerObject.GetComponent<PlayerView>();
        playerObject.transform.position = Vector3.zero;
    }

    private void InitializeJewelBoard(int initialJewelCount)
    {
        initialJewelCount = Mathf.Clamp(initialJewelCount, 1, 9);
        List<int> allJewels = new List<int>();
        for (int i = 1; i <= 10; i++)
        {
            allJewels.Add(i);
        }

        collectedJewelViews = new List<JewelView>();
        allJewelView = new Stack<JewelView>();

        for (int i = 0; i < initialJewelCount; i++)
        {
            int randomIndex = Random.Range(0, allJewels.Count);
            int jewelID = allJewels[randomIndex];
            allJewels.RemoveAt(randomIndex);

            CreateAndAddJewel(GetJewelBoardPosition(jewelID), jewelID);
        }
    }

    private void CalculateBounds()
    {
        minBounds = new Vector3(-10, -4.0f, 0.0f);
        maxBounds = new Vector3(10, 1.5f, 0.0f);
    }

    public Vector3 GetBackgroundMinBounds()
    {
        return minBounds;
    }

    public Vector3 GetBackgroundMaxBounds()
    {
        return maxBounds;
    }

    private void CreateBoxObjects()
    {
        int numberOfBoxes = 10;
        for (int i = 0; i < numberOfBoxes; i++)
        {
            Vector3 newPosition = GenerateUniquePosition();
            GameObject newBox = Instantiate(boxPrefab, newPosition, Quaternion.identity, objectInParent);
            newBox.GetComponent<BoxView>().SetData(i + 1);
        }
    }

    private void CreateAndAddJewel(Vector2 createdPos, int jewelID)
    {
        JewelView newJewel = CreateJewelObject(createdPos, jewelID);
        collectedJewelViews.Add(newJewel);
        newJewel.gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    public JewelView CreateJewelObject(Vector2 createdPos, int jewelID)
    {
        GameObject newJewel = Instantiate(jewelPrefab, objectInParent);
        newJewel.transform.position = createdPos;
        JewelView jewelView = newJewel.GetComponent<JewelView>();
        jewelView.SetData(jewelID);
        allJewelView.Push(jewelView);
        return jewelView;
    }

    public void PlayJewelAnimation(JewelView jewel)
    {
        if (jewel == null)
        {
            if (allJewelView.Count > 0)
            {
                jewel = allJewelView.Peek();
            }
            else
            {
                Debug.LogWarning("No jewels available in the stack to animate.");
                return;
            }
        }
        jewel.transform.localScale = Vector3.zero;
        jewel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        Vector2 initPos = jewel.transform.position;
        Vector2 randomPos = initPos + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        jewel.transform.DOMove(randomPos, 1f).SetEase(Ease.OutBounce);
    }

    private Vector3 GenerateUniquePosition()
    {
        Vector3 newPosition;
        int maxAttempts = 100;
        int attempts = 0;

        do
        {
            float x = Random.Range(minBounds.x, maxBounds.x);
            float y = Random.Range(minBounds.y, maxBounds.y);
            newPosition = new Vector3(x, y, 0.0f);
            attempts++;
        } while (IsPositionUsed(newPosition) && attempts < maxAttempts);

        if (attempts < maxAttempts)
        {
            usedPositions.Add(newPosition);
        }
        else
        {
            Debug.LogWarning("Unable to find a unique position for new box.");
        }

        return newPosition;
    }

    private bool IsPositionUsed(Vector3 position)
    {
        float tolerance = 2.0f;
        foreach (Vector3 usedPosition in usedPositions)
        {
            if (Vector3.Distance(position, usedPosition) < tolerance)
            {
                return true;
            }
        }
        return false;
    }

    public Vector2 GetJewelBoardPosition(int jewelID)
    {
        return new Vector2(jewelBoardInitPosition.x + jewelBoardXPositionDiff * (jewelID - 1), jewelBoardInitPosition.y);
    }

    public void AddJewel(JewelView jewelView)
    {
        collectedJewelViews.Add(jewelView);
        jewelView.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        CheckWinCondition();
    }

    public bool CheckWinCondition()
    {
        if (collectedJewelViews.Count == 10) return true;
        else return false;
    }

    public void EndStage()
    {
        OnDispose();
        this.GetComponent<PlayerInput>().SwitchCurrentActionMap("UIInput");
        endButton.SetActive(true);
    }


    public bool IsCheckPlayerJewel(int jewelID)
    {
        foreach (JewelView jewelView in collectedJewelViews)
        {
            if (jewelView.GetJewelID() == jewelID)
            {
                return true;
            }
        }
        return false;
    }

    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnDispose()
    {
        foreach (JewelView jewelView in collectedJewelViews)
        {
            jewelView.OnDispose();
        }
        collectedJewelViews.Clear();
        collectedJewelViews = null;
        allJewelView.Clear();
        allJewelView = null;
        foreach (Transform child in objectInParent)
        {
            Destroy(child.gameObject);
        }
        Resources.UnloadUnusedAssets();
    }

    //Input System
    //UI 조작 관련 입력
    public void OnUIAButton()
    {
        if (playButton.gameObject.activeSelf)
        {
            this.GetComponent<PlayerInput>().SwitchCurrentActionMap("PlayerInput");
            Initialize();
        }
        else if (endButton.gameObject.activeSelf) 
        {
            this.GetComponent<PlayerInput>().SwitchCurrentActionMap("PlayerInput");
            Initialize();
        }
    }
    public void OnUIBButton() 
    {
        EndGame();
    }

    //캐릭터 조작 관련 입력
    public void OnMove(InputValue value)
    {
        if (playerObject != null && playerView != null) 
        {
            playerView.OnMove(value);
        }
    }
    public void OnInteraction()
    {
        if (playerObject != null && playerView != null)
        {
            playerView.OnInteraction();
        }
    }

}
