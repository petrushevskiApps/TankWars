using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FindAmmo : GoapAction 
{

	bool completed = false;

	private IGoap agent;
	private Memory agentMemory;
	private NavigationSystem agentNavigation;

	public FindAmmo() 
	{
		name = "FindAmmo";

		AddPrecondition(StateKeys.AMMO_AMOUNT, false);
		AddPrecondition(StateKeys.AMMO_DETECTED, false);
		AddPrecondition(StateKeys.ENEMY_DETECTED, false);

		AddEffect(StateKeys.AMMO_DETECTED, true);

	}
	private void Start()
	{
		agent = GetComponent<IGoap>();
		agentMemory = agent.GetMemory();
		agentNavigation = agent.GetNavigation();
	}
	public override void Reset()
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

	public override bool CheckPreconditions (GameObject agentGo)
	{
		if(agentMemory.Enemies.IsAnyValidDetected())
		{
			agentNavigation.AbortMoving();
			return false;
		}

		if(agentMemory.AmmoPacks.IsAnyValidDetected())
		{
			agentNavigation.AbortMoving();
			return true;
		}

		return true;
	}

	public override void ExecuteAction(GameObject agent, Action success, Action fail)
	{
		Debug.Log($"<color=green> {agent.name} Perform Action: {name}</color>");

		if (agentMemory.AmmoPacks.IsAnyValidDetected())
		{
			completed = true;
			ExitAction(success);
			return;
		}

		ExitAction(fail);
	}
	protected override void ExitAction(Action exitAction)
	{
		agentNavigation.InvalidateTarget();
		exitAction?.Invoke();
	}

}
