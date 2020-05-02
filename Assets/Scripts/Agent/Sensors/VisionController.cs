using GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VisionController : MonoBehaviour
{

    public EnemyDetected EnemyDetectedEvent = new EnemyDetected();
    public EnemyLost EnemyLostEvent = new EnemyLost();

    public PackageEvent AmmoPackDetected = new PackageEvent();
    public PackageEvent AmmoPackLost = new PackageEvent();
    public PackageEvent HealthPackDetected = new PackageEvent();
    public PackageEvent HealthPackLost = new PackageEvent();

    // On Trigger is called when detectable object
    // is in radius range of visibility.
    private void OnTriggerStay(Collider other)
    {
        float angle = Utilities.GetAngle(transform.parent.gameObject, other.gameObject);

        if(IsInFront(angle) && IsVisible(other.gameObject))
        {
            CheckDetectableType(other.gameObject);
        }
    }

    // OnTriggerExit is called when detectable 
    // object is lost of sight.
    private void OnTriggerExit(Collider other)
    {
        GameObject target = other.gameObject;
        
        if (target.CompareTag("Tank"))
        {
            if (IsEnemy(target.GetComponent<Tank>()))
            {
                EnemyLostEvent.Invoke(target);
            }
        }
        else if(target.CompareTag("Pickable"))
        {
            AmmoPackLost.Invoke(target);
        }
    }

    // Check if the target object is
    // in front of the current object.
    private bool IsInFront(float angle)
    {
        return angle < 90f;
    }

    private bool IsDetectable(float angle)
    {
        return angle < 22;
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

    // Check the type of object which was detected
    private void CheckDetectableType(GameObject target)
    {
        if (target.CompareTag("Tank"))
        {
            Tank targetTank = target.GetComponent<Tank>();

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
        else if(target.CompareTag("AmmoPack"))
        {
            Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.green);

            AmmoPackDetected.Invoke(target);
        }
        else if (target.CompareTag("HealthPack"))
        {
            Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.green);

            HealthPackDetected.Invoke(target);
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

    public class PackageEvent : UnityEvent<GameObject>
    {

    }

}
