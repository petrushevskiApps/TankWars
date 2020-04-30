using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : GoapAction 
{

	private bool completed = false;

	private IGoap agent;
	private Memory agentMemory;
	private NavigationSystem agentNavigation;

	public Patrol() 
	{
		name = "Patrol";

		AddPrecondition(StateKeys.ENEMY_DETECTED, false);
		AddPrecondition(StateKeys.AMMO_AMOUNT, true);

		AddEffect(GoalKeys.PATROL, true);
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
		agentNavigation.SetTarget();
		target = agentNavigation.GetTarget();
	}


	public override bool CheckProceduralPrecondition (GameObject agent)
	{	
		return !agentMemory.Enemies.IsAnyValidDetected();
	}
	
	public override void ExecuteAction(GameObject agent, Action success, Action fail)
	{
		completed = true;
		ExitAction(success);
	}
	
	protected override void ExitAction(Action exitAction)
	{
		agentNavigation.InvalidateTarget();
		exitAction?.Invoke();
	}


}
