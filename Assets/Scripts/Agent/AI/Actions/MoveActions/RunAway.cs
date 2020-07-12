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

			return Mathf.Clamp(cost, minimumCost + 0.1f, Mathf.Infinity);
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
			ExitAction(actionCompleted, 0f);
		}
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
			agent.BoostOff();
			RestartAction();
		}
		else
		{
			// If the agent arrives at the target location and isn't
			// under attack action is completed. 
			ExitAction(actionCompleted, 0f);
		}
	}
	
	protected override void ExitAction(Action ExitAction, float invalidateTime)
	{
		agent.BoostOff();
		base.ExitAction(ExitAction, invalidateTime);
	}


	protected override void RegisterListeners()
	{
		base.RegisterListeners();

		agent.Memory.HidingSpots.OnDetected.AddListener(ReplanningAbort);
		agent.Memory.HealthPacks.OnDetected.AddListener(ReplanningAbort);
		agent.Memory.AmmoPacks.OnDetected.AddListener(ReplanningAbort);
	}

	protected override void UnregisterListeners()
	{
		base.UnregisterListeners();

		agent.Memory.HidingSpots.OnDetected.RemoveListener(ReplanningAbort);
		agent.Memory.HealthPacks.OnDetected.RemoveListener(ReplanningAbort);
		agent.Memory.AmmoPacks.OnDetected.RemoveListener(ReplanningAbort);
	}
}
