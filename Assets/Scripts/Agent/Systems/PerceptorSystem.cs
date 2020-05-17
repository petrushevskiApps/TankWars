using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PerceptorSystem : MonoBehaviour
{
    [SerializeField] private List<Sensor> sensors = new List<Sensor>();

    public PlayerEvent OnFriendlyDetected = new PlayerEvent();
    public PlayerEvent OnEnemyDetected = new PlayerEvent();
    public PlayerEvent OnEnemyLost = new PlayerEvent();

    public PackageEvent OnAmmoPackDetected = new PackageEvent();
    public PackageEvent OnAmmoPackLost = new PackageEvent();
    public PackageEvent OnHealthPackDetected = new PackageEvent();
    public PackageEvent OnHealthPackLost = new PackageEvent();

    public HiddingSpot OnHiddingSpotDetected = new HiddingSpot();

    private void Awake()
    {
        RegisterToSensors();
    }
    private void OnDestroy()
    {
        UnregisterToSensors();
    }

    private void RegisterToSensors()
    {
        foreach(Sensor sensor in sensors)
        {
            sensor.OnVisibleDetected.AddListener(CheckVisibleDetected);
            sensor.OnInisibleDetected.AddListener(CheckInvisibleDetected);
            sensor.OnLost.AddListener(CheckLost);
        }
    }

    private void UnregisterToSensors()
    {
        foreach (Sensor sensor in sensors)
        {
            sensor.OnVisibleDetected.RemoveListener(CheckVisibleDetected);
            sensor.OnInisibleDetected.RemoveListener(CheckInvisibleDetected);
            sensor.OnLost.RemoveListener(CheckLost);
        }
    }

    private void CheckLost(GameObject target)
    {
        if (target.CompareTag("Tank"))
        {
            if (IsEnemy(target.GetComponent<Player>()))
            {
                OnEnemyLost.Invoke(target);
            }
            else
            {
                OnFriendlyDetected.Invoke(target);
            }
        }
        else if (target.CompareTag("AmmoPack"))
        {
            OnAmmoPackLost.Invoke(target);
        }
        else if (target.CompareTag("HealthPack"))
        {
            OnHealthPackLost.Invoke(target);
        }
    }

    // Check the type of object which was detected
    private void CheckVisibleDetected(GameObject target)
    {
        if (target.CompareTag("Tank"))
        {
            Player targetTank = target.GetComponent<Player>();

            if (IsEnemy(targetTank))
            {
                Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.red);
                OnEnemyDetected.Invoke(target);
            }
            else
            {
                Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.blue);
                OnFriendlyDetected.Invoke(target);
            }
        }
        else if (target.CompareTag("AmmoPack"))
        {
            Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.green);

            OnAmmoPackDetected.Invoke(target);
        }
        else if (target.CompareTag("HealthPack"))
        {
            Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.green);

            OnHealthPackDetected.Invoke(target);
        }
    }
    private void CheckInvisibleDetected(GameObject detected)
    {
        if (detected.CompareTag("HidingSpot"))
        {
            Debug.DrawRay(transform.position, detected.transform.position - transform.position, Color.magenta);

            OnHiddingSpotDetected.Invoke(detected);
        }
    }

    private bool IsEnemy(Player targetTank)
    {
        int targetID = targetTank.GetTeamID();
        int ID = transform.parent.GetComponent<Player>().GetTeamID();
        return ID != targetID;
    }

    public class PlayerEvent : UnityEvent<GameObject>
    {

    }

    public class PackageEvent : UnityEvent<GameObject>
    {

    }
    public class HiddingSpot : UnityEvent<GameObject>
    {

    }
}
