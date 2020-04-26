using UnityEngine;

public class Pickable : MonoBehaviour
{
    private float timeIn = 0;
    private float timeToCollect = 2f;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Tank"))
        {
            if (timeIn >= timeToCollect)
            {
                Destroy(transform.parent.gameObject);
            }
            else
            {
                timeIn += Time.deltaTime;
            }
        }
    }
 
}