using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class RunAway : MoveAction 
{
	public UnityEvent OnRunAwayEntered = new UnityEvent();
	public UnityEvent OnRunAwayExecuted = new UnityEvent();

	public RunAway() 
	{
		actionName = "RunAway";

		AddPrecondition(StateKeys.UNDER_ATTACK, true);

		AddEffect(StateKeys.UNDER_ATTACK, false);

	}
	public override bool TestProceduralPreconditions()
	{
		return !agentMemory.IsAmmoAvailable() || !agentMemory.IsHealthAvailable();
	}

	public override void SetActionTarget()
	{
		Vector3 missileDirection = agentMemory.missileDirection;
		
		if(missileDirection != Vector3.zero)
		{
			agentNavigation.SetRunFromTarget(missileDirection);
			target = agentNavigation.GetNavigationTarget();
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
		OnRunAwayEntered.Invoke();
	}
	/*
	 * If the agent arrives at the target location and isn't
	 * under attack action is completed. 
	 * If the agent is still under attack get new
	 * target location to run away.
	 */
	public override void ExecuteAction(GameObject agent)
	{
		if(agentMemory.IsUnderAttack)
		{
			RestartAction();
		}
		else
		{
			OnRunAwayExecuted.Invoke();
			ExitAction(actionCompleted);
		}
	}

	protected override void AddListeners()
	{
		agent.GetPerceptor().OnHidingSpotDetected.AddListener(HidingSpotDetected);

	}
	protected override void RemoveListeners()
	{
		agent.GetPerceptor().OnHidingSpotDetected.RemoveListener(HidingSpotDetected);
	}

	
	private void HidingSpotDetected(GameObject hiddingSpot)
	{
		if (!agentMemory.IsUnderAttack)
		{
			if (!agentMemory.IsHealthAvailable() || !agentMemory.IsAmmoAvailable())
			{
				ExitAction(actionCompleted);
			}
		}
		
	}

}
