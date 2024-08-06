using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionUnitView : UnitView
{
    protected Transform focusObjectTransform;
    public virtual void SetData(int id)
    {
        focusObjectTransform = this.transform.GetChild(0);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (focusObjectTransform != null)
        {
            focusObjectTransform.gameObject.SetActive(true);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (focusObjectTransform != null)
        {
            focusObjectTransform.gameObject.SetActive(false);
        }
    }

    public virtual void OnDispose()
    {

    }
}
