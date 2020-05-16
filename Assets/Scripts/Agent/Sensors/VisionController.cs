using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VisionController : Sensor
{

    // On Trigger is called when detectable object
    // is in radius range of visibility.
    private void OnTriggerStay(Collider other)
    {
        float angle = Utilities.GetAngle(transform.parent.gameObject, other.gameObject);

        if(IsInFront(angle))
        {
            if(IsVisible(other.gameObject))
            {
                OnVisibleDetected.Invoke(other.gameObject);
            }
            else
            {
                OnInisibleDetected.Invoke(other.gameObject);
                
            }
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
    private bool IsInFront(float angle)
    {
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
