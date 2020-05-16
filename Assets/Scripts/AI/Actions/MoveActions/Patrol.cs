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
		agent.GetPerceptor().OnEnemyDetected.AddListener(OnEnemyDetected);
		agent.GetPerceptor().OnAmmoPackDetected.AddListener(OnAmmoDetected);
		agent.GetPerceptor().OnHealthPackDetected.AddListener(OnHealthDetected);
		agent.GetPerceptor().OnHiddingSpotDetected.AddListener(OnHidingSpotDetected);

	}

	private void RemoveListeners()
	{
		agent.GetPerceptor().OnEnemyDetected.RemoveListener(OnEnemyDetected);
		agent.GetPerceptor().OnAmmoPackDetected.RemoveListener(OnAmmoDetected);
		agent.GetPerceptor().OnHealthPackDetected.RemoveListener(OnHealthDetected);
		agent.GetPerceptor().OnHiddingSpotDetected.RemoveListener(OnHidingSpotDetected);
	}

	private void OnEnemyDetected(GameObject enemy)
	{
		if (agentMemory.Enemies.IsAnyValidDetected())
		{
			ExitAction(actionFailed);
		}
	}

	private void OnAmmoDetected(GameObject ammo)
	{
		if (!agent.GetInventory().IsAmmoAvailable())
		{
			ExitAction(actionCompleted);
		}
	}
	private void OnHealthDetected(GameObject health)
	{
		if (!agent.GetInventory().IsHealthAvailable())
		{
			ExitAction(actionCompleted); 
		}
	}
	private void OnHidingSpotDetected(GameObject hiddingSpot)
	{
		if (!agent.GetInventory().IsHealthAvailable() && !agent.GetInventory().IsAmmoAvailable())
		{
			ExitAction(actionCompleted);
		}
	}
}
