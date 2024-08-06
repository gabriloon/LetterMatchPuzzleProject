using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceUtil
{
    private static Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
    private static Dictionary<string, GameObject> gameObjectCache = new Dictionary<string, GameObject>();

    public static void SetJewelNormalSprite(SpriteRenderer spriteRenderer, int id)
    {
        string path = "Images/InGame/Jewel/Jewel_" + id;
        spriteRenderer.sprite = LoadSprite(path);
    }

    public static void SetJewelFocusSprite(SpriteRenderer spriteRenderer, int id)
    {
        string path = "Images/InGame/Jewel/Jewel_" + id + "_L";
        spriteRenderer.sprite = LoadSprite(path);
    }

    public static GameObject GetPlayerObject()
    {
        string path = "Prefabs/Unit/PlayerCharacter";
        return LoadGameObject(path);
    }
    public static GameObject GetBoxObject()
    {
        string path = "Prefabs/Unit/Box";
        return LoadGameObject(path);
    }
    public static GameObject GetJewelObject()
    {
        string path = "Prefabs/Unit/Jewel";
        return LoadGameObject(path);
    }

    private static Sprite LoadSprite(string path)
    {
        if (spriteCache.TryGetValue(path, out Sprite sprite))return sprite;
        sprite = Resources.Load<Sprite>(path);
        if (sprite == null) Debug.LogWarning("ResourcesUtil's LoadSprite Result: Null");
        else spriteCache[path] = sprite;
        spriteCache[path] = sprite;
        return sprite;
    }

    private static GameObject LoadGameObject(string path)
    {
        if (gameObjectCache.TryGetValue(path, out GameObject gameObject))return gameObject;
        gameObject = Resources.Load<GameObject>(path);
        if (gameObject == null) Debug.LogWarning("ResourcesUtil's GameObject Result: Null");
        else gameObjectCache[path] = gameObject;
        return gameObject;
    }
}