using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : MoveAction 
{
	public Patrol() 
	{
		actionName = "Patrol";

		AddPrecondition(StateKeys.ENEMY_DETECTED, false);

		AddEffect(StateKeys.PATROL, true);
	}
	
	public override void SetActionTarget()
	{
		agentNavigation.SetTarget();
		target = agentNavigation.GetNavigationTarget();
	}


	public override bool CheckPreconditions (GameObject agentGO)
	{
		return true;
	}

	public override void EnterAction(Action success, Action fail)
	{
		base.EnterAction(success, fail);
		
		AddListeners();
	}
	
	protected override void ExitAction(Action ExitAction)
	{
		RemoveListeners();
		base.ExitAction(ExitAction);
	}

	private void AddListeners()
	{
		agentMemory.Enemies.OnDetected.AddListener(OnEnemyDetected);
		agentMemory.AmmoPacks.OnDetected.AddListener(OnAmmoDetected);
		agentMemory.HealthPacks.OnDetected.AddListener(OnHealthDetected);
		agentMemory.HidingSpots.OnDetected.AddListener(OnHidingSpotDetected);

	}

	private void RemoveListeners()
	{
		agentMemory.Enemies.OnDetected.RemoveListener(OnEnemyDetected);
		agentMemory.AmmoPacks.OnDetected.RemoveListener(OnAmmoDetected);
		agentMemory.HealthPacks.OnDetected.RemoveListener(OnHealthDetected);
		agentMemory.HidingSpots.OnDetected.RemoveListener(OnHidingSpotDetected);
	}

	private void OnEnemyDetected()
	{
		if (agentMemory.Enemies.IsAnyValidDetected())
		{
			ExitAction(actionFailed);
		}
	}
	private void OnAmmoDetected()
	{
		if (!agent.GetInventory().IsAmmoAvailable())
		{
			ExitAction(actionCompleted);
		}
	}
	private void OnHealthDetected()
	{
		if (!agent.GetInventory().IsHealthAvailable())
		{
			ExitAction(actionCompleted);
		}
	}
	private void OnHidingSpotDetected()
	{
		if (!agent.GetInventory().IsHealthAvailable() && !agent.GetInventory().IsAmmoAvailable())
		{
			ExitAction(actionCompleted);
		}
	}
}
