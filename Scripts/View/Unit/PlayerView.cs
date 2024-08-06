using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Spine.Unity;
using Spine;

public class PlayerView : UnitView
{
    [SerializeField] private float moveSpeed = 4f;
    private Vector2 moveInput;
    private SkeletonAnimation skeletonAnimation;
    private string idleAnimation = "Idle_F";
    private string runAnimation = "Run_S";
    private string resultAnimation = "Result";
    private PlayerAnimationState currAnimState;
    private bool isControllable = false;
    private GameObject interactionObject;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        unitType = UnitType.Player;
        currAnimState = PlayerAnimationState.Idle;
    }

    void Update()
    {
        if (isControllable)
        {
            Move();
            UpdateAnimation();
        }
    }
    public void SetControllable(bool isControll) 
    {
        isControllable = isControll;
    }

    private void Move()
    {
        if (moveInput == Vector2.zero) return;

        Vector3 move = new Vector3(moveInput.x, moveInput.y, 0.0f) * moveSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + move;
        Vector3 minBounds = InGameManager.instance.GetBackgroundMinBounds();
        Vector3 maxBounds = InGameManager.instance.GetBackgroundMaxBounds();

        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

        transform.position = newPosition;
    }
    public void SetAnimation(PlayerAnimationState setAnimState)
    {
        if (currAnimState == setAnimState) return;

        switch (setAnimState)
        {
            case PlayerAnimationState.Idle:
                PlayAnimation(idleAnimation, true);
                break;
            case PlayerAnimationState.Run:
                PlayAnimation(runAnimation, true);
                break;
            case PlayerAnimationState.Happy:
                var trackEntry = PlayAnimation(resultAnimation, false);
                trackEntry.Complete += delegate {
                    SetAnimation(PlayerAnimationState.Idle);
                    isControllable = true;
                };
                break;
            case PlayerAnimationState.Clear:
                isControllable = false;
                var clearTrackEntry = skeletonAnimation.AnimationState.SetAnimation(0, resultAnimation, false);
                clearTrackEntry.Complete += delegate {
                    InGameManager.instance.EndStage();
                };
                break;
        }
        currAnimState = setAnimState;
    }

    private TrackEntry PlayAnimation(string animationName, bool loop)
    {
        return skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
    }

    private void UpdateAnimation()
    {
        if (moveInput != Vector2.zero)
        {
            SetAnimation(PlayerAnimationState.Run);
        }
        else
        {
            SetAnimation(PlayerAnimationState.Idle);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        interactionObject = other.gameObject;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (interactionObject == other.gameObject)
        {
            interactionObject = null;
        }
    }

    /* Input System */
    public void OnMove(InputValue value)
    {
        if (!isControllable) return;
        moveInput = value.Get<Vector2>();
        if (moveInput.x != 0)
        {
            skeletonAnimation.skeleton.ScaleX = moveInput.x > 0 ? -1 : 1;
        }
    }

    public void OnInteraction()
    {
        if (!isControllable) return;
        if (interactionObject == null)
        {
            Debug.Log("No interaction object");
            return;
        }
        Debug.Log("Interacting Object: " + interactionObject.name);
        moveInput = Vector2.zero;
        var unitView = interactionObject.GetComponent<UnitView>();
        if (unitView == null) return;
        switch (unitView.GetType())
        {
            case UnitType.Box:
                interactionObject.GetComponent<BoxView>().OpenBox();
                break;
            case UnitType.Jewel:
                isControllable = false;
                interactionObject.GetComponent<JewelView>().PickupJewel(() => 
                {
                    if(InGameManager.instance.CheckWinCondition()) SetAnimation(PlayerAnimationState.Clear);
                    else SetAnimation(PlayerAnimationState.Happy); 
                });
                break;
        }
    }
}
