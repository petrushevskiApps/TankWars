using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Agent
{
    

    private new void Awake()
    {
        RegisterListeners();
        base.Awake();
    }
    private void OnDestroy()
    {
        UnregisterListeners();
    }
    private void RegisterListeners()
    {
        InputController.OnFirePressed.AddListener(Fire);
        InputController.OnCollecting.AddListener(collectController.CollectPickable);
    }
    private void UnregisterListeners()
    {
        InputController.OnFirePressed.RemoveListener(Fire);
        InputController.OnCollecting.RemoveListener(collectController.CollectPickable);
    }
    
    private void Fire()
    {
        weapon.FireBullet();
    }

}
