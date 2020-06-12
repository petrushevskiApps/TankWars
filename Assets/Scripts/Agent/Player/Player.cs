using Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Agent
{
    private PlayerMovement movementController;

    private new void Awake()
    {
        RegisterListeners();
        base.Awake();

        movementController = GetComponent<PlayerMovement>();
    }
    private void OnDestroy()
    {
        UnregisterListeners();
    }
    private void RegisterListeners()
    {
        InputController.OnMovementAxis.AddListener(OnMovement);
        InputController.OnTurningAxis.AddListener(OnTurning);

        InputController.OnFirePressed.AddListener(Fire);
        InputController.OnCollecting.AddListener(collectController.CollectPickable);

        InputController.OnBoostStart.AddListener(BoostOn);
        InputController.OnBoostEnd.AddListener(BoostOff);
    }
    private void UnregisterListeners()
    {
        InputController.OnMovementAxis.RemoveListener(OnMovement);
        InputController.OnTurningAxis.RemoveListener(OnTurning);

        InputController.OnFirePressed.RemoveListener(Fire);
        InputController.OnCollecting.RemoveListener(collectController.CollectPickable);

        InputController.OnBoostStart.RemoveListener(BoostOn);
        InputController.OnBoostEnd.RemoveListener(BoostOff);
    }

    private void OnMovement(float inputValue)
    {
        movementController.UpdateMovement(inputValue);

        if(inputValue > 0)
        {
            VisualSystem.Particles.PlayDrivingParticles();
        }
        else
        {
            VisualSystem.Particles.StopDrivingParticles();
        }
    }

    private void OnTurning(float inputValue)
    {
        movementController.UpdateTurning(inputValue);
    }

    private void BoostOn()
    {
        if(Inventory.SpeedBoost.Amount > 0)
        {
            movementController.BoostSpeed();
            VisualSystem.Particles.PlayDrivingParticles();
            Inventory.SpeedBoost.DecreaseStart();
        }
        else
        {
            movementController.ResetSpeed();
        }
    }
    private void BoostOff()
    {
        movementController.ResetSpeed();
        Inventory.SpeedBoost.IncreaseStart();
    }

    private void Fire()
    {
        weaponController.FireBullet();
    }

}
