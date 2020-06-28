using System;
using UnityEngine;

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

		
		if (enemy != null)
		{
			// If enemy is detected we count cost with function
			float enemyHealth = enemy.Inventory.Health.Amount;
			float enemyAmmo = enemy.Inventory.Ammo.Amount * 10;

			float agentHealth = agent.Inventory.Health.Amount;
			float agentAmmo = agent.Inventory.Ammo.Amount * 10;
			

			float cost = (agentAmmo - enemyHealth) + (agentHealth - enemyAmmo);

			return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
		}
		else
		{
			return Mathf.Infinity;
		}
		
		
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
		if(agent.Memory.IsUnderAttack)
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
		agent.Memory.Enemies.OnDetected.AddListener(EnemyDetected);
		agent.Memory.Enemies.OnRemoved.AddListener(EnemyLost);

		agent.Memory.HidingSpots.OnDetected.AddListener(HidingSpotDetected);
		agent.Memory.HealthPacks.OnDetected.AddListener(HealthDetected);
		agent.Memory.AmmoPacks.OnDetected.AddListener(AmmoDetected);
	}

	protected override void RemoveListeners()
	{
		agent.Memory.Enemies.OnDetected.RemoveListener(EnemyDetected);
		agent.Memory.Enemies.OnRemoved.RemoveListener(EnemyLost);

		agent.Memory.HidingSpots.OnDetected.RemoveListener(HidingSpotDetected);
		agent.Memory.HealthPacks.OnDetected.RemoveListener(HealthDetected);
		agent.Memory.AmmoPacks.OnDetected.RemoveListener(AmmoDetected);
	}

	private void EnemyDetected()
	{
		ExitAction(actionFailed);
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
		ExitAction(actionFailed);
	}

	private void AmmoDetected()
	{
		ExitAction(actionFailed);
	}

	// If agent is no more under attack and
	// detects valid hiding spot re-plan
	private void HidingSpotDetected()
	{
		ExitAction(actionFailed);
	}

	
}
