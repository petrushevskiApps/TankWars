using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VisionController : MonoBehaviour
{
    public UnityEvent EnemyDetected = new UnityEvent();
    public UnityEvent PickableDetected = new UnityEvent();

    
    // On Trigger is called when detectable object
    // is in radius range of visibility.
    private void OnTriggerStay(Collider other)
    {
        float angle = Utilities.GetAngle(gameObject, other.gameObject);

        if(IsInFront(angle) && IsVisible(other.gameObject))
        {
            CheckDetectableType(other.gameObject);

            Debug.Log("Sensor (" + this.gameObject.name + ") Detected: " + other.gameObject.name + " With angle of: " + angle);
           
        }
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
                Debug.Log("Raycast: Did Hit");
                return true;
            }
        }

        Debug.Log("Raycast: Did not Hit");
        return false;
    }

    // Check the type of object that was detected
    private void CheckDetectableType(GameObject target)
    {
        Tank targetTank = target.GetComponent<Tank>();
        
        if (targetTank != null)
        {
            int targetID = targetTank.GetTeamID();
            int ID = transform.parent.GetComponent<Tank>().GetTeamID();
            if (ID != targetID)
            {
                Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.red);
                EnemyDetected.Invoke();
            }
            else
            {
                Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.blue);
            }
        }
        else if(target.GetComponent<Pickable>() != null)
        {
            Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.green);
            PickableDetected.Invoke();
        }
    }

    

    


    
}
