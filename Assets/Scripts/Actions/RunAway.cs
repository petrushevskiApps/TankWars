using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunAway : GoapAction 
{

	bool completed = false;
	
	private IGoap agent;
	private Memory agentMemory;
	private NavigationSystem agentNavigation;

	public RunAway() 
	{
		name = "RunAway";

		AddPrecondition(StateKeys.ENEMY_DETECTED, true);
		AddPrecondition(StateKeys.AMMO_AMOUNT, false);

		AddEffect(StateKeys.ENEMY_DETECTED, false);
	}
	private void Start()
	{
		agent = GetComponent<IGoap>();
		agentMemory = agent.GetMemory();
		agentNavigation = agent.GetNavigation();
	}
	public override void Reset ()
	{
		target = null;
		completed = false;
	}
	
	public override bool IsActionDone ()
	{
		return completed;
	}

	public override void SetActionTarget()
	{
		GameObject enemy = agentMemory.Enemies.GetDetected();
		agentNavigation.SetRunFromTarget(enemy);
		target = agentNavigation.GetTarget();
	}


	public override bool CheckPreconditions (GameObject agent)
	{	
		
		return true;
	}
	
	public override void ExecuteAction(GameObject agent, Action success, Action fail)
	{
		if(!agentMemory.Enemies.IsAnyValidDetected())
		{
			completed = true;
			ExitAction(success);
		}
		else
		{
			ExitAction(fail);
		}
	}

	protected override void ExitAction(Action exitAction)
	{
		agentNavigation.InvalidateTarget();
		exitAction?.Invoke();
	}
}
