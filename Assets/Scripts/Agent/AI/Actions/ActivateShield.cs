using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActivateShield : GoapAction 
{
	protected AIAgent agent;

	public ActivateShield() 
	{
		AddPrecondition(StateKeys.UNDER_ATTACK, true);

		AddEffect(StateKeys.UNDER_ATTACK, false);
	}
	private void Start()
	{
		agent = GetComponent<AIAgent>();
	}

	public override float GetCost()
	{
		Agent enemy = agent.Memory.Enemies.GetSortedDetected()?.GetComponent<Agent>();

		float enemyAmmoTime = 0;

		//if (enemy != null)
		//{
		//	// If enemy is detected we count cost with function
		//	enemyAmmoTime = enemy.Inventory.Ammo.Amount * 0.5f;
		//}
		//else
		//{
		//	enemyAmmoTime = 
		//}

		enemyAmmoTime = enemy.Inventory.Ammo.Amount * 0.5f;

		float agentHealthTime = (agent.Inventory.Health.Amount / 10) * 0.5f;

		float invertedShieldTime = 10 - agent.Inventory.Shield.Amount;

		float cost = invertedShieldTime + (agentHealthTime - enemyAmmoTime);

		return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
	}

	
	public override void SetActionTarget()
	{
		// No target for this action
	}
	public override void InvalidTargetLocation()
	{
		// No target for this action
	}

	public override void EnterAction(Action Success, Action Fail, Action Reset)
	{
		actionCompleted = Success;
		actionFailed = Fail;
		actionReset = Reset;
	}

	public override void ExecuteAction()
	{
		if(agent.Inventory.Shield.Amount > 3)
		{
			agent.ToggleShield();
			ExitAction(actionCompleted);
		}
		else
		{
			ExitAction(actionFailed);
		}
	}

	protected override void ExitAction(Action ExitAction)
	{
		IsActionDone = true;
		ExitAction?.Invoke();
	}
	
}
