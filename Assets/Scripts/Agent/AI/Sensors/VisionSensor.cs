using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VisionSensor : Sensor
{

    // On Trigger is called when detectable object
    // is in radius range of visibility.
    private void OnTriggerStay(Collider detected)
    {
        if(IsInFront(detected))
        {
            if(IsVisible(detected.gameObject))
            {
                OnVisibleDetected.Invoke(detected.gameObject, true);
            }
            else
            {
                OnDetected.Invoke(detected.gameObject, true);
            }
        }
    }

    // OnTriggerExit is called when detectable 
    // object is lost of sight.
    private void OnTriggerExit(Collider other)
    {
        OnLost.Invoke(other.gameObject, true);
    }

    // Check if the target object is
    // in front of the current object.
    private bool IsInFront(Collider detected)
    {
        float angle = Utilities.GetAngle(transform.parent.gameObject, detected.gameObject);
        return angle < 90f;
    }

    // Check if the target object is not behind
    // other objects in scene
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
