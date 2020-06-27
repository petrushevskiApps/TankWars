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
		AddPrecondition(StateKeys.ENEMY_DETECTED, true);
		AddPrecondition(StateKeys.HEALTH_FULL, false);
		AddPrecondition(StateKeys.AMMO_FULL, false);

		AddEffect(StateKeys.ENEMY_DETECTED, false);
	}

	public override float GetCost()
	{
		Agent enemy = agent.Memory.Enemies.GetSortedDetected()?.GetComponent<Agent>();
		
		float enemyHealthTime = 0;
		float enemyAmmoTime = 0;
		
		if (enemy != null)
		{
			// If enemy is detected we count cost with function
			enemyHealthTime = (enemy.Inventory.Health.Amount / 10) * 0.5f;
			enemyAmmoTime   = enemy.Inventory.Ammo.Amount * 0.5f;
		}
		else
		{
			// If enemy is not yet detected we count cost as maximum possible
			// by taking the capacity values from agent.
			enemyHealthTime = (agent.Inventory.Health.Capacity / 10) * 0.5f;
			enemyAmmoTime = agent.Inventory.Ammo.Capacity * 0.5f;
		}
		
		float agentAmmoTime = agent.Inventory.Ammo.Amount * 0.5f;
		float agentHealthTime = (agent.Inventory.Health.Amount / 10) * 0.5f;
		float shieldTime = agent.Inventory.Shield.Amount;

		float cost = shieldTime + ((agentAmmoTime - enemyHealthTime) + (agentHealthTime - enemyAmmoTime));

		return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
	}

	public override void SetActionTarget()
	{
		GameObject enemy = agent.Memory.Enemies.GetSortedDetected();

		if(enemy != null)
		{
			agent.Navigation.SetRunAwayTarget(enemy.transform.position);
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
	

	public override void ExecuteAction()
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
