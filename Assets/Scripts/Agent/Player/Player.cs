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
        InputController.OnSpacePressed.AddListener(Fire);
    }
    private void UnregisterListeners()
    {
        InputController.OnSpacePressed.RemoveListener(Fire);
    }

    private void Fire()
    {
        weapon.FireBullet();
    }

}
