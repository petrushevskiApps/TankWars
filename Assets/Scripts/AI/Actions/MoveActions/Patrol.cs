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

		AddPrecondition(StateKeys.UNDER_ATTACK, false);

		AddEffect(StateKeys.PATROL, true);
	}
	
	public override void SetActionTarget()
	{
		agentNavigation.SetTarget();
		target = agentNavigation.GetNavigationTarget();
	}

	public override void InvalidTargetLocation()
	{
		SetActionTarget();
	}

	public override bool TestProceduralPreconditions()
	{
		return true;
	}

	public override void ExecuteAction(GameObject agent)
	{
		RestartAction();
	}


	protected override void AddListeners()
	{
		agent.GetPerceptor().OnEnemyDetected.AddListener(EnemyDetected);
		agent.GetPerceptor().OnAmmoPackDetected.AddListener(AmmoDetected);
		agent.GetPerceptor().OnHealthPackDetected.AddListener(HealthDetected);
		agent.GetPerceptor().OnHidingSpotDetected.AddListener(HidingSpotDetected);
		agent.GetPerceptor().OnUnderAttack.AddListener(UnderAttack);
	}
	
	protected override void RemoveListeners()
	{
		agent.GetPerceptor().OnEnemyDetected.RemoveListener(EnemyDetected);
		agent.GetPerceptor().OnAmmoPackDetected.RemoveListener(AmmoDetected);
		agent.GetPerceptor().OnHealthPackDetected.RemoveListener(HealthDetected);
		agent.GetPerceptor().OnHidingSpotDetected.RemoveListener(HidingSpotDetected);
		agent.GetPerceptor().OnUnderAttack.RemoveListener(UnderAttack);
	}

	private void EnemyDetected(GameObject enemy)
	{
		if(agentMemory.IsAmmoAvailable())
		{
			ExitAction(actionCompleted);
		}
	}
	private void UnderAttack(GameObject arg0)
	{
		ExitAction(actionFailed);
	}

	private void HealthDetected(GameObject health)
	{
		if (!agentMemory.IsHealthAvailable())
		{
			ExitAction(actionCompleted);
		}
	}
	private void AmmoDetected(GameObject ammo)
	{
		if (!agentMemory.IsAmmoAvailable())
		{
			ExitAction(actionCompleted);
		}
	}
	
	private void HidingSpotDetected(GameObject hiddingSpot)
	{
		if(agentMemory.HidingSpots.IsAnyValidDetected())
		{
			if (!agentMemory.IsHealthAvailable() || !agentMemory.IsAmmoAvailable())
			{
				ExitAction(actionCompleted);
			}
		}
	}

	
}
