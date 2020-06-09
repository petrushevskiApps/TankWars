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

		AddPrecondition(StateKeys.UNDER_ATTACK, true);

		AddEffect(StateKeys.UNDER_ATTACK, false);

	}
	public override bool CheckProceduralPreconditions()
	{
		return !agent.Memory.IsAmmoAvailable() || !agent.Memory.IsHealthAvailable();
	}

	public override void SetActionTarget()
	{
		Vector3 missileDirection = agent.Memory.MissileDirection;
		
		if(missileDirection != Vector3.zero)
		{
			agent.Navigation.SetTarget(missileDirection, true);
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
	}
	/*
	 * If the agent arrives at the target location and isn't
	 * under attack action is completed. 
	 * If the agent is still under attack get new
	 * target location to run away.
	 */
	public override void ExecuteAction(GameObject agent)
	{
		if(this.agent.Memory.IsUnderAttack)
		{
			RestartAction();
		}
		else
		{
			ExitAction(actionCompleted);
		}
	}

	protected override void AddListeners()
	{
		agent.Memory.HidingSpots.OnDetected.AddListener(HidingSpotDetected);

	}
	protected override void RemoveListeners()
	{
		agent.Memory.HidingSpots.OnDetected.RemoveListener(HidingSpotDetected);
	}

	
	private void HidingSpotDetected()
	{
		if (agent.Memory.HidingSpots.IsAnyValidDetected() && !agent.Memory.IsUnderAttack)
		{
			if (CheckProceduralPreconditions())
			{
				ExitAction(actionCompleted);
			}
		}
		
	}

}
