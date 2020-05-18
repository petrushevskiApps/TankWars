using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Inventory
{
    [SerializeField] private float healthAmount = 100;
    [SerializeField] private float healthCapacity = 100;

    [SerializeField] private int ammoAmount = 10;
    [SerializeField] private int ammoCapacity = 10;
    
    public InventoryStatus AmmoStatus { get; private set; }
    public InventoryStatus HealthStatus { get; private set; }

    public HealthChangeEvent OnHealthChange = new HealthChangeEvent();

    public void Initialize()
    {
        SetAmmoStatus();
        SetHealthStatus();
    }

    public void IncreaseAmmo(int ammo)
    {
        ammoAmount = Mathf.Clamp(ammoAmount + ammo, 0, ammoCapacity);
        SetAmmoStatus();
    }

    

    public void DecreaseAmmo()
    {
        ammoAmount = Mathf.Clamp(ammoAmount-1, 0, ammoCapacity);
        SetAmmoStatus();
    }
    public int GetAmmo()
    {
        return ammoAmount;
    }
    public void SetAmmoStatus()
    {
        if (ammoAmount <= 0)
        {
            AmmoStatus = InventoryStatus.Empty;
        }
        else if(ammoAmount > 0 && ammoAmount <= 5)
        {
            AmmoStatus = InventoryStatus.Low;
        }
        else if (ammoAmount > 5 && ammoAmount < ammoCapacity)
        {
            AmmoStatus = InventoryStatus.Medium;
        }
        else if(ammoAmount >= ammoCapacity)
        {
            AmmoStatus = InventoryStatus.Full;
        }
    }

    public void IncreaseHealth(float amount)
    {
        healthAmount = Mathf.Clamp(healthAmount + amount, 0, healthCapacity);
        OnHealthChange.Invoke(healthAmount);
        SetHealthStatus();
    }
    public void DecreaseHealth(float amount)
    {
        healthAmount = Mathf.Clamp(healthAmount - amount, 0, healthCapacity);
        OnHealthChange.Invoke(healthAmount);
        SetHealthStatus();
    }
    public float GetHealth()
    {
        return healthAmount;
    }

    
    public void SetHealthStatus()
    {
        if (healthAmount < 50)
        {
            HealthStatus = InventoryStatus.Low;
        }
        else if (healthAmount >= 50 && healthAmount < healthCapacity)
        {
            HealthStatus = InventoryStatus.Medium;
        }
        else
        {
            HealthStatus = InventoryStatus.Full;
        }
    }

    public class HealthChangeEvent : UnityEvent<float>
    {

    }
}
