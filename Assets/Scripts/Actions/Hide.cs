using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hide : GoapAction 
{
	private IGoap agent;
	private Memory agentMemory;
	private NavigationSystem agentNavigation;

	private bool completed = false;

	public Hide() 
	{
		name = "Hide";

		AddPrecondition(StateKeys.HEALTH_AMOUNT, false);
		AddPrecondition(StateKeys.AMMO_AMOUNT, false);
		AddPrecondition(StateKeys.HIDING_SPOT_DETECTED, true);

		AddEffect(StateKeys.HEALTH_AMOUNT, true);
		AddEffect(StateKeys.AMMO_AMOUNT, true);
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
		if (agentMemory.HidingSpots.IsAnyValidDetected())
		{
			target = agentMemory.HidingSpots.GetDetected();
			agentNavigation.SetTarget(target);
		}
	}
	

	public override bool CheckPreconditions (GameObject agentGo)
	{
		if(agentMemory.Enemies.IsAnyValidDetected())
		{
			return false;
		}
		return true;
	}
	
	public override void ExecuteAction(GameObject agent, Action success, Action fail)
	{
		Debug.Log($"<color=green> {gameObject.name} Perform Action: {name}</color>");
		StartCoroutine(Regenerate(success, fail));
	}
	
	protected override void ExitAction(Action exitAction)
	{
		agentNavigation.InvalidateTarget();
		exitAction?.Invoke();
	}

	IEnumerator Regenerate(Action succes, Action fail)
	{
		while(agent.GetInventory().GetHealth() < 100)
		{
			agent.GetInventory().IncreaseHealth(10);
			yield return new WaitForSeconds(1f);
		}
		while(agent.GetInventory().ammoAmount < agent.GetInventory().ammoCapacity)
		{
			agent.GetInventory().AddAmmo(2);
			yield return new WaitForSeconds(1f);
		}
		completed = true;
		ExitAction(succes);
	}


}
