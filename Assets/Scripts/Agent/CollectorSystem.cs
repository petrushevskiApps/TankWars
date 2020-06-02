using UnityEngine;

/*
 * This class is used by agents
 * for collecting pickables in world.
 */
public class CollectorSystem : MonoBehaviour
{
    private Pickable pickable;

    public bool IsPickableReady => pickable != null;

    private void OnTriggerEnter(Collider other)
    {
       if (other.gameObject.layer == LayerMask.NameToLayer("Pickable"))
        {
            SetPickable(other.gameObject.transform.parent.GetComponent<Pickable>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pickable"))
        {
            ResetPickable(other.gameObject);
        }
    }

    private void SetPickable(Pickable pickable)
    {
        this.pickable = pickable;
        this.pickable.RegisterOnDestroy(ResetPickable);
    }

    private void ResetPickable(GameObject pickableObject)
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
