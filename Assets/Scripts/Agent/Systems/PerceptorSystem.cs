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

    public HiddingSpot OnHidingSpotDetected = new HiddingSpot();
    public HiddingSpot OnHidingSpotLost = new HiddingSpot();

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
            sensor.OnDetected.AddListener(CheckVisibleDetected);
            sensor.OnLost.AddListener(CheckLost);
        }
    }

    private void UnregisterToSensors()
    {
        foreach (Sensor sensor in sensors)
        {
            sensor.OnDetected.RemoveListener(CheckVisibleDetected);
            sensor.OnLost.RemoveListener(CheckLost);
        }
    }

    private void CheckLost(GameObject target, bool isVisible)
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
        else if (target.CompareTag("HidingSpot"))
        {
            OnHidingSpotLost.Invoke(target);
        }
    }

    // Check the type of object which was detected
    private void CheckVisibleDetected(GameObject detected, bool isVisible)
    {
        if (detected.CompareTag("Tank") && isVisible)
        {
            Player targetTank = detected.GetComponent<Player>();

            if (IsEnemy(targetTank))
            {
                Debug.DrawRay(transform.position, detected.transform.position - transform.position, Color.red);
                OnEnemyDetected.Invoke(detected);
            }
            else
            {
                Debug.DrawRay(transform.position, detected.transform.position - transform.position, Color.blue);
                OnFriendlyDetected.Invoke(detected);
            }
        }
        else if(detected.CompareTag("HidingSpot"))
        {
            Debug.DrawRay(transform.position, detected.transform.position - transform.position, Color.magenta);

            OnHidingSpotDetected.Invoke(detected);
        }
        else if (detected.CompareTag("AmmoPack") && isVisible)
        {
            Debug.DrawRay(transform.position, detected.transform.position - transform.position, Color.green);

            OnAmmoPackDetected.Invoke(detected);
        }
        else if (detected.CompareTag("HealthPack") && isVisible)
        {
            Debug.DrawRay(transform.position, detected.transform.position - transform.position, Color.green);

            OnHealthPackDetected.Invoke(detected);
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
