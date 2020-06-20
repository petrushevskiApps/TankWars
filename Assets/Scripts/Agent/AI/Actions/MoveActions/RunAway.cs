using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class RunAway : MoveAction 
{
	public RunAway() 
	{
		actionName = "RunAway";

		AddPrecondition(StateKeys.ENEMY_DETECTED, true);

		AddEffect(StateKeys.ENEMY_DETECTED, false);

	}
	//public override bool CheckProceduralPreconditions()
	//{
	//	return !agent.Memory.IsAmmoAvailable() || !agent.Memory.IsHealthAvailable();
	//}

	public override float GetCost()
	{ 
		float TTE = timeToExecute;
		float E = GetEnemyCost(agent.Memory.Enemies);
		float IH = GetInventoryCost(agent.Inventory.Health.Status, false);
		float IA = GetInventoryCost(agent.Inventory.Ammo.Status, false);

		float cost = TTE - E - IH - IA;
		return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
	}

	

	public override void SetActionTarget()
	{
		//Vector3 missileDirection = agent.Memory.MissileDirection;
		GameObject enemy = agent.Memory.Enemies.GetSortedDetected();

		if(enemy != null && enemy.transform.position != Vector3.zero)
		{
			agent.Navigation.SetTarget(enemy.transform.position, true);
			target = agent.Navigation.Target;
		}
		else
		{
			ExitAction(actionCompleted);
		}
	}
	public override void InvalidTargetLocation()
	{
		SetActionTarget();
	}

	public override void EnterAction(Action Success, Action Fail, Action Reset)
	{
		base.EnterAction(Success, Fail, Reset);
		agent.BoostOn();
	}
	

	public override void ExecuteAction(GameObject agent)
	{
		if(this.agent.Memory.IsUnderAttack)
		{
			 // If the agent is still under attack get new
			 // target location to run away.
			RestartAction();
		}
		else
		{
			// If the agent arrives at the target location and isn't
			// under attack action is completed. 
			ExitAction(actionCompleted);
		}
	}
	protected override void ExitAction(Action ExitAction)
	{
		base.ExitAction(ExitAction);
		agent.BoostOff();
	}
	protected override void AddListeners()
	{
		agent.Memory.HidingSpots.OnDetected.AddListener(HidingSpotDetected);
		agent.Memory.HealthPacks.OnDetected.AddListener(HealthDetected);
		agent.Memory.AmmoPacks.OnDetected.AddListener(AmmoDetected);
		agent.Memory.Enemies.OnRemoved.AddListener(EnemyLost);

	}
	protected override void RemoveListeners()
	{
		agent.Memory.HidingSpots.OnDetected.RemoveListener(HidingSpotDetected);
		agent.Memory.HealthPacks.OnDetected.RemoveListener(HealthDetected);
		agent.Memory.AmmoPacks.OnDetected.RemoveListener(AmmoDetected);
		agent.Memory.Enemies.OnRemoved.RemoveListener(EnemyLost);
	}

	private void EnemyLost()
	{
		if(agent.Memory.Enemies.GetValidDetectedCount() == 0)
		{
			if(!agent.Memory.IsUnderAttack)
			{
				ExitAction(actionCompleted);
			}
		}
	}

	private void HealthDetected()
	{
		if(!agent.Memory.IsUnderAttack && agent.Memory.Enemies.GetValidDetectedCount() == 0)
		{
			if(!agent.Memory.IsHealthFull())
			{
				ExitAction(actionCompleted);
			}
		}
	}

	private void AmmoDetected()
	{
		if (!agent.Memory.IsUnderAttack && agent.Memory.Enemies.GetValidDetectedCount() == 0)
		{
			if (!agent.Memory.IsAmmoFull())
			{
				ExitAction(actionCompleted);
			}
		}
	}

	// If agent is no more under attack and
	// detects valid hiding spot complete action
	// and re-plan
	private void HidingSpotDetected()
	{
		if (agent.Memory.HidingSpots.IsAnyValidDetected() && !agent.Memory.IsUnderAttack)
		{
			ExitAction(actionCompleted);
		}
		
	}

}
