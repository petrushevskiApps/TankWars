using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunAway : MoveAction 
{	
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
		GameObject enemy = agentMemory.Enemies.GetDetected();
		
		if(enemy != null)
		{
			agentNavigation.SetRunFromTarget(enemy);
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
