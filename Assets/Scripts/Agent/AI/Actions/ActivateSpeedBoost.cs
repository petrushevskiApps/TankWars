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
		actionName = "ActivateSpeedBoost";

		//AddPrecondition(StateKeys.HIDING_SPOT_DETECTED, true);

		//AddEffect(StateKeys.HEALTH_FULL, true);
		//AddEffect(StateKeys.AMMO_FULL, true);

		
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
		float TTE = timeToExecute;
		float IH = agent.Inventory.Health.GetInvertedCost();
		float IA = agent.Inventory.Ammo.GetInvertedCost();
		float ISB = agent.Inventory.SpeedBoost.GetInvertedCost();

		float cost = TTE + IH + IA - ISB;
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

		IsInRange = true;
	}

	public override void ExecuteAction(GameObject agent)
	{
		if(this.agent.Inventory.SpeedBoost.Amount > 0)
		{
			this.agent.BoostOn();
		}
	}

	protected override void ExitAction(Action ExitAction)
	{
		IsActionDone = true;
		ExitAction?.Invoke();
	}
	
}
