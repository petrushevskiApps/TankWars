using Complete;
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

    public MissileEvent OnUnderAttack = new MissileEvent();
    public MissileEvent OnFriendlyFire = new MissileEvent();

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
            sensor.OnDetected.AddListener(CheckDetected);
            sensor.OnLost.AddListener(CheckLost);
        }
    }

    private void UnregisterToSensors()
    {
        foreach (Sensor sensor in sensors)
        {
            sensor.OnVisibleDetected.RemoveListener(CheckVisibleDetected);
            sensor.OnDetected.RemoveListener(CheckDetected);
            sensor.OnLost.RemoveListener(CheckLost);
        }
    }

    private void CheckLost(GameObject detected)
    {
        if (detected.CompareTag("Tank"))
        {
            Agent targetTank = detected.GetComponent<Agent>();

            if (IsEnemy(targetTank.GetTeamID()))
            {
                OnEnemyLost.Invoke(detected);
            }
        }
        else if (detected.CompareTag("HidingSpot"))
        {
            OnHidingSpotLost.Invoke(detected);
        }
    }

    // Check the type of object which was detected
    private void CheckVisibleDetected(GameObject detected)
    {
        if (detected.CompareTag("Tank"))
        {
            Agent targetTank = detected.GetComponent<Agent>();

            if (IsEnemy(targetTank.GetTeamID()))
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
        
        else if (detected.CompareTag("AmmoPack"))
        {
            Debug.DrawRay(transform.position, detected.transform.position - transform.position, Color.green);

            OnAmmoPackDetected.Invoke(detected);
        }
        else if (detected.CompareTag("HealthPack"))
        {
            Debug.DrawRay(transform.position, detected.transform.position - transform.position, Color.green);

            OnHealthPackDetected.Invoke(detected);
        }
    }
    private void CheckDetected(GameObject detected)
    {
        if (detected.CompareTag("HidingSpot"))
        {
            Debug.DrawRay(transform.position, detected.transform.position - transform.position, Color.magenta);

            OnHidingSpotDetected.Invoke(detected);
        }
        else if (detected.CompareTag("Missile"))
        {
            Shell missile = detected.GetComponent<Shell>();

            if(missile != null)
            {
                if (IsEnemy(missile.GetOwnerTeam()))
                {
                    OnUnderAttack.Invoke(detected);
                }
                else if(!IsOwner(missile.GetOwnerName()))
                {
                    OnFriendlyDetected.Invoke(detected);
                }
            }
            
        }

    }
    private bool IsEnemy(int targetTeamID)
    {
        int ID = transform.parent.GetComponent<Agent>().GetTeamID();
        return ID != targetTeamID;
    }
    private bool IsOwner(string targetName)
    {
        return transform.parent.name.Equals(targetName);
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

    public class MissileEvent : UnityEvent<GameObject>
    {

    }
}
