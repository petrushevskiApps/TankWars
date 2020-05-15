using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TacticalPosition : GoapAction 
{

	private bool completed = false;

	private IGoap agent;
	private Memory agentMemory;
	private NavigationSystem agentNavigation;

	public TacticalPosition() 
	{
		actionName = "TacticalPosition";

		AddPrecondition(StateKeys.ENEMY_DETECTED, true);
		AddPrecondition(StateKeys.IN_SHOOTING_RANGE, false);
		AddPrecondition(StateKeys.AMMO_AMOUNT, true);
		AddPrecondition(StateKeys.HEALTH_AMOUNT, true);

		AddEffect(StateKeys.IN_SHOOTING_RANGE, true);
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

		agentNavigation.SetTarget(CalculateTacticalPosition());
		target = agentNavigation.GetTarget();
	}


	public override bool CheckPreconditions (GameObject agent)
	{	
		return true;
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

	private Vector3 CalculateTacticalPosition()
	{
		return gameObject.transform.position + (gameObject.transform.right * 5);
	}

}
