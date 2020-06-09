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
		agent.Navigation.SetTarget();
		target = agent.Navigation.Target;
	}

	public override void InvalidTargetLocation()
	{
		SetActionTarget();
	}

	public override bool CheckProceduralPreconditions()
	{
		return true;
	}

	public override void EnterAction(Action Success, Action Fail, Action Reset)
	{
		base.EnterAction(Success, Fail, Reset);
	}

	public override void ExecuteAction(GameObject agent)
	{
		RestartAction();
	}

	protected override void AddListeners()
	{
		agent.Memory.Enemies.OnDetected.AddListener(EnemyDetected);
		agent.Memory.AmmoPacks.OnDetected.AddListener(AmmoDetected);
		agent.Memory.HealthPacks.OnDetected.AddListener(HealthDetected);
		agent.Memory.HidingSpots.OnDetected.AddListener(HidingSpotDetected);
		agent.Sensors.OnUnderAttack.AddListener(UnderAttack);
	}
	
	protected override void RemoveListeners()
	{
		agent.Memory.Enemies.OnDetected.RemoveListener(EnemyDetected);
		agent.Memory.AmmoPacks.OnDetected.RemoveListener(AmmoDetected);
		agent.Memory.HealthPacks.OnDetected.RemoveListener(HealthDetected);
		agent.Memory.HidingSpots.OnDetected.RemoveListener(HidingSpotDetected);
		agent.Sensors.OnUnderAttack.RemoveListener(UnderAttack);
	}

	private void EnemyDetected()
	{
		ExitAction(actionCompleted);
	}

	private void UnderAttack(GameObject arg0)
	{
		ExitAction(actionFailed);
	}

	private void HealthDetected()
	{
		if (!agent.Memory.IsHealthAvailable())
		{
			ExitAction(actionCompleted);
		}
	}
	private void AmmoDetected()
	{
		if (!agent.Memory.IsAmmoAvailable())
		{
			ExitAction(actionCompleted);
		}
	}
	
	private void HidingSpotDetected()
	{
		if (!agent.Memory.IsHealthAvailable() || !agent.Memory.IsAmmoAvailable())
		{
			ExitAction(actionCompleted);
		}
	}

	
}
