using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunAway : MoveAction 
{	
	public RunAway() 
	{
		actionName = "RunAway";

		AddPrecondition(StateKeys.ENEMY_DETECTED, true);

		AddEffect(StateKeys.ENEMY_DETECTED, false);
	}

	public override void SetActionTarget()
	{
		GameObject enemy = agentMemory.Enemies.GetDetected();
		
		if(enemy != null)
		{
			agentNavigation.SetRunFromTarget(enemy);
			target = agentNavigation.GetNavigationTarget();
		}
	}
	public override bool TestProceduralPreconditions()
	{
		return !agentMemory.IsAmmoAvailable() || !agentMemory.IsHealthAvailable();
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
		agent.GetPerceptor().OnEnemyLost.AddListener(EnemyLost);
		agent.GetPerceptor().OnAmmoPackDetected.AddListener(OnAmmoDetected);
		agent.GetPerceptor().OnHealthPackDetected.AddListener(OnHealthDetected);
		agent.GetPerceptor().OnHidingSpotDetected.AddListener(OnHidingSpotDetected);

	}

	private void OnAmmoDetected(GameObject ammo)
	{
		if (!agentMemory.IsAmmoAvailable() && !agentMemory.IsUnderAttack)
		{
			ExitAction(actionCompleted);
		}
	}
	private void OnHealthDetected(GameObject health)
	{
		if (!agentMemory.IsHealthAvailable() && !agentMemory.IsUnderAttack)
		{
			ExitAction(actionCompleted);
		}
	}
	private void OnHidingSpotDetected(GameObject hiddingSpot)
	{
		if (!agentMemory.IsHealthAvailable() && !agentMemory.IsAmmoAvailable())
		{
			if(!agentMemory.IsUnderAttack)
			{
				ExitAction(actionCompleted);
			}
			
		}
	}
	private void EnemyLost(GameObject enemy) 
	{
		if(!agentMemory.Enemies.IsAnyValidDetected())
		{
			ExitAction(actionCompleted);
		}
	}

	private void RemoveListeners()
	{
		agent.GetPerceptor().OnEnemyLost.RemoveListener(EnemyLost);
		agent.GetPerceptor().OnAmmoPackDetected.RemoveListener(OnAmmoDetected);
		agent.GetPerceptor().OnHealthPackDetected.RemoveListener(OnHealthDetected);
		agent.GetPerceptor().OnHidingSpotDetected.RemoveListener(OnHidingSpotDetected);
	}

}
