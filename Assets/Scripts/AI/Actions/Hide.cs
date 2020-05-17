using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hide : GoapAction 
{
	private IGoap agent;
	private MemorySystem agentMemory;
	private NavigationSystem agentNavigation;


	public Hide() 
	{
		actionName = "Hide";

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

	public override void ResetAction()
	{
		base.ResetAction();
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
		return true;
	}
	
	public override void EnterAction(Action success, Action fail)
	{
		actionCompleted = success;
		actionFailed = fail;
		SetActionTarget();
	}

	public override void ExecuteAction(GameObject agent)
	{
		Debug.Log($"<color=green> {gameObject.name} Perform Action: {actionName}</color>");
		StartCoroutine(Regenerate());
	}
	
	protected override void ExitAction(Action exitAction)
	{
		IsActionDone = true;
		target = null;
		agentNavigation.InvalidateTarget();
		exitAction?.Invoke();
	}

	IEnumerator Regenerate()
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
		
		ExitAction(actionCompleted);
	}


}
