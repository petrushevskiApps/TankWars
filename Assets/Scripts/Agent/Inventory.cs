using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Inventory
{
    [SerializeField] private float healthAmount = 100;
    [SerializeField] private float healthCapacity = 100;

    public int ammoAmount = 10;
    public int ammoCapacity = 10;

    public HealthChangeEvent OnHealthChange = new HealthChangeEvent();


    public bool IsAmmoAvailable()
    {
        return ammoAmount > 0;
    }
    public void AddAmmo(int ammo)
    {
        ammoAmount = Mathf.Clamp(ammoAmount + ammo, 0, ammoCapacity);
    }

    public void DecreaseAmmo()
    {
        ammoAmount--;
    }

    public void IncreaseHealth(float amount)
    {
        healthAmount = Mathf.Clamp(healthAmount + amount, 0, healthCapacity);
        OnHealthChange.Invoke(healthAmount);
    }
    public void DecreaseHealth(float amount)
    {
        healthAmount = Mathf.Clamp(healthAmount - amount, 0, healthCapacity);
        OnHealthChange.Invoke(healthAmount);
    }
    public float GetHealth()
    {
        return healthAmount;
    }

    public bool IsHealthAvailable()
    {
        return healthAmount > 30;
    }

    public class HealthChangeEvent : UnityEvent<float>
    {

    }
}
