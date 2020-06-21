using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActivateSpeedBoost : GoapAction 
{
	protected AIAgent agent;

	public ActivateSpeedBoost() 
	{
		AddPrecondition(StateKeys.UNDER_ATTACK, true);

		AddEffect(StateKeys.UNDER_ATTACK, false);

	}
	private void Start()
	{
		agent = GetComponent<AIAgent>();
	}
	public override bool CheckProceduralPreconditions()
	{
		// Check if the agent is under attack at
		// the moment of planning 
		//return agent.Memory.IsUnderAttack;
		return true;
	}

	public override float GetCost()
	{
		float IH = agent.Inventory.Health.GetCost();
		float IA = agent.Inventory.Ammo.GetCost();
		float ISB = agent.Inventory.SpeedBoost.GetInvertedCost();

		float cost = ISB - IH - IA;
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
		if(this.agent.Inventory.SpeedBoost.Amount > 0)
		{
			this.agent.BoostOn();
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
