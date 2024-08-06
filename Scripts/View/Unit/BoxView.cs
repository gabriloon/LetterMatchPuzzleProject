using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxView : InteractionUnitView
{
    private int jewelID = 0;
    public Sprite openBoxImage;
    public Sprite openBoxFocusImage;
    public override void SetData(int id)
    {
        base.SetData(id);
        unitType = UnitType.Box;
        jewelID = id;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
    }

    public void OpenBox()
    {
        if (openBoxImage == null) return;
        this.GetComponent<SpriteRenderer>().sprite = openBoxImage;
        focusObjectTransform.GetComponent<SpriteRenderer>().sprite = openBoxFocusImage;
        InGameManager.instance.CreateJewelObject(this.transform.position, jewelID);
        InGameManager.instance.PlayJewelAnimation(null);
        OnDispose();
    }

    public override void OnDispose()
    {
        base.OnDispose();
        openBoxImage = null;
        openBoxFocusImage = null;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.DOFade(0, 1f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
        Destroy(this.GetComponent<BoxCollider2D>());
    }
}
