using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class JewelView : InteractionUnitView
{
    private int jewelID = 0;
    public override void SetData(int id)
    {
        base.SetData(id);
        unitType = UnitType.Jewel;
        jewelID = id;

        SpriteRenderer spriteNormalRenderer = this.GetComponent<SpriteRenderer>();
        SpriteRenderer spriteFocusRenderer = focusObjectTransform.GetComponent<SpriteRenderer>();
        if (spriteNormalRenderer != null) ResourceUtil.SetJewelNormalSprite(spriteNormalRenderer, jewelID);
        if (spriteFocusRenderer != null) ResourceUtil.SetJewelFocusSprite(spriteFocusRenderer, jewelID);
    }

    public int GetJewelID() 
    {
        return jewelID; 
    }

    public void PickupJewel(System.Action onComplete) 
    {
        bool isHave = InGameManager.instance.IsCheckPlayerJewel(jewelID);
        Vector2 targetPosition = InGameManager.instance.GetJewelBoardPosition(jewelID);
        this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        OnTriggerExit2D(null);
        transform.DOMove(targetPosition, 1.0f).OnComplete(() =>
        {
            if (isHave)
            {
                Vector3 offScreenPosition = new Vector3(20, 20, 0); // 화면 밖 임의의 위치
                transform.DOMove(offScreenPosition, 1.0f).OnComplete(() => Destroy(gameObject));
            }
            else 
            {
                InGameManager.instance.AddJewel(this); // 플레이어에게 보석 추가
            }
            onComplete.Invoke();
        });
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
    }

    public override void OnDispose()
    {
        base.OnDispose();
        Destroy(gameObject);
    }
}
