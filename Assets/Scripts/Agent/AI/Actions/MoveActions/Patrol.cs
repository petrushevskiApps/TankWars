using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Patrol : MoveAction 
{
	public UnityEvent OnPatrolExecuted = new UnityEvent();

	public Patrol() 
	{
		actionName = "Patrol";

		AddPrecondition(StateKeys.UNDER_ATTACK, false);

		AddEffect(StateKeys.PATROL, true);
	}
	
	public override void SetActionTarget()
	{
		agentNavigation.SetTarget();
		target = agentNavigation.Target;
	}

	public override void InvalidTargetLocation()
	{
		SetActionTarget();
	}

	public override bool TestProceduralPreconditions()
	{
		return true;
	}

	public override void EnterAction(Action Success, Action Fail, Action Reset)
	{
		base.EnterAction(Success, Fail, Reset);
	}
	public override void ExecuteAction(GameObject agent)
	{
		CallForHelp();
		RestartAction();
	}

	private void CallForHelp()
	{
		if (!agentMemory.IsHealthAvailable())
		{
			agent.Communication.BroadcastNeedHealth();
		}
		else if (!agentMemory.IsAmmoAvailable())
		{
			agent.Communication.BroadcastNeedAmmo();
		}
		else
		{
			// Health and Ammo Full and No Enemies found
			OnPatrolExecuted.Invoke();
		}
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
