using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VisionController : MonoBehaviour
{
    public EnemyDetected EnemyDetectedEvent = new EnemyDetected();
    public EnemyLost EnemyLostEvent = new EnemyLost();

    public PackDetected AmmoPackDetected = new PackDetected();
    public PackDetected HealthPackDetected = new PackDetected();

    // On Trigger is called when detectable object
    // is in radius range of visibility.
    private void OnTriggerStay(Collider other)
    {
        float angle = Utilities.GetAngle(gameObject, other.gameObject);

        if(IsInFront(angle) && IsVisible(other.gameObject))
        {
            CheckDetectableType(other.gameObject);

            //Debug.Log("Sensor (" + this.gameObject.name + ") Detected: " + other.gameObject.name + " With angle of: " + angle);
           
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Tank targetTank = other.gameObject.GetComponent<Tank>();

        if (targetTank != null)
        {
            EnemyLostEvent.Invoke(other.gameObject);
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
                return true;
            }
        }

        return false;
    }

    // Check the type of object that was detected
    private void CheckDetectableType(GameObject target)
    {
        Tank targetTank = target.GetComponent<Tank>();

        if (targetTank != null)
        {
            if (IsEnemy(targetTank))
            {
                Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.red);
                EnemyDetectedEvent.Invoke(target);
            }
            else
            {
                Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.blue);
            }
        }
        else if(target.GetComponent<Pickable>() != null)
        {
            Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.green);

            Pickable pickable = target.GetComponent<Pickable>();
            if (pickable.GetType() == typeof(AmmoPack))
            {
                AmmoPackDetected.Invoke(target);
            }
            else if(pickable.GetType() == typeof(HealthPack))
            {
                HealthPackDetected.Invoke(target);
            }
        }
    }
    private bool IsEnemy(Tank targetTank)
    {
        int targetID = targetTank.agentMemory.GetTeamID();
        int ID = transform.parent.GetComponent<Tank>().agentMemory.GetTeamID();
        return ID != targetID;
    }
    
    public class EnemyDetected : UnityEvent<GameObject>
    {

    }
    public class EnemyLost : UnityEvent<GameObject>
    {

    }
    public class PackDetected : UnityEvent<GameObject>
    {

    }


}
