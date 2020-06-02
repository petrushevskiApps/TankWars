using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollectController : MonoBehaviour
{
    private Pickable pickable;

    public bool IsPickableReady => pickable != null;

    private void OnTriggerEnter(Collider other)
    {
       if (other.gameObject.layer == LayerMask.NameToLayer("Pickable"))
        {
            PickableDetected(other.gameObject.transform.parent.GetComponent<Pickable>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pickable"))
        {
            PickableLost(other.gameObject);
        }
    }

    private void PickableDetected(Pickable pickable)
    {
        this.pickable = pickable;
        this.pickable.RegisterOnDestroy(PickableLost);
    }

    private void PickableLost(GameObject pickableObject)
    {
        if(IsPickableReady)
        {
            pickable.StopCollecting();
            pickable = null;
        }
    }

    public void CollectPickable(bool isCollecting)
    {
        if (IsPickableReady)
        {
            if(isCollecting)
            {
                pickable.StartCollecting(transform.parent.gameObject.GetComponent<ICollector>());
            }
            else
            {
                pickable.StopCollecting();
            }
        }
    }

}
