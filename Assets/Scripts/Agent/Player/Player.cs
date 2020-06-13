using Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Agent
{
    private PlayerNavigation navigation;

    private new void Awake()
    {
        RegisterListeners();
        base.Awake();

        navigation = (PlayerNavigation)navigationController;
    }
    private void OnDestroy()
    {
        UnregisterListeners();
    }
    private void RegisterListeners()
    {
        InputController.OnMovementAxis.AddListener(OnMovementInput);
        InputController.OnTurningAxis.AddListener(OnTurningInput);

        InputController.OnFirePressed.AddListener(Fire);
        InputController.OnCollecting.AddListener(collectController.CollectPickable);

        InputController.OnBoostStart.AddListener(BoostOn);
        InputController.OnBoostEnd.AddListener(BoostOff);

        InputController.OnShieldToggle.AddListener(ToggleShield);
    }
    private void UnregisterListeners()
    {
        InputController.OnMovementAxis.RemoveListener(OnMovementInput);
        InputController.OnTurningAxis.RemoveListener(OnTurningInput);

        InputController.OnFirePressed.RemoveListener(Fire);
        InputController.OnCollecting.RemoveListener(collectController.CollectPickable);

        InputController.OnBoostStart.RemoveListener(BoostOn);
        InputController.OnBoostEnd.RemoveListener(BoostOff);

        InputController.OnShieldToggle.RemoveListener(ToggleShield);
    }

    private void OnMovementInput(float inputValue)
    {
        navigation.UpdateMovement(inputValue);
    }

    private void OnTurningInput(float inputValue)
    {
        navigation.UpdateTurning(inputValue);
    }

    private void Fire()
    {
        weaponController.FireBullet();
    }

}
