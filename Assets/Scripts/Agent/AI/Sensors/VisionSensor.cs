using GOAP;
using UnityEngine;

public class VisionSensor : Sensor
{

    // On Trigger is called when detectable object
    // is in sight ( in front ).
    private void OnTriggerStay(Collider detected)
    {
        if(IsInFront(detected))
        {
            OnDetected.Invoke(detected.gameObject, (IsVisible(detected.gameObject)));
        }
    }

    // OnTriggerExit is called when detectable 
    // object is lost of sight.
    private void OnTriggerExit(Collider other)
    {
        OnLost.Invoke(other.gameObject);
    }

    // Check if the target object is
    // in front of the current object.
    private bool IsInFront(Collider detected)
    {
        float angle = Utilities.GetAngle(transform.parent.gameObject, detected.gameObject);
        return angle < 90f;
    }

    // Check if the target object is not behind
    // other objects in scene ( Line of sight )
    private bool IsVisible(GameObject target)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Utilities.GetDirection(gameObject, target), out hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject == target)
            {
                return true;
            }
        }

        return false;
    }

    
   
    
    
}
