using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollectAmmo : GoapAction 
{

	private IGoap agent;
	private Memory agentMemory;
	private NavigationSystem agentNavigation;

	private Coroutine Update;

	public CollectAmmo() 
	{
		actionName = "CollectAmmo";

		AddPrecondition(StateKeys.AMMO_AMOUNT, false);
		AddPrecondition(StateKeys.AMMO_DETECTED, true);

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
		if (agentMemory.AmmoPacks.IsAnyValidDetected())
		{
			target = agentMemory.AmmoPacks.GetDetected();
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
		
		Update = StartCoroutine(ActionUpdate());
	}

	public override void ExecuteAction(GameObject agent)
	{
		Debug.Log($"<color=green> {gameObject.name} Perform Action: {actionName}</color>");

		if(target != null && target.activeSelf)
		{
			StartCoroutine(WaitAction());
		}
		else
		{
			ExitAction(actionFailed);
		}
	}
	IEnumerator WaitAction()
	{
		yield return new WaitUntil(() => agent.GetInventory().IsAmmoAvailable());
		agentMemory.AmmoPacks.RemoveDetected(target);
		ExitAction(actionCompleted);
	}
	

	IEnumerator ActionUpdate()
	{
		while(true)
		{
			if (agentMemory.Enemies.IsAnyValidDetected())
			{
				GameObject enemy = agentMemory.Enemies.GetDetected();

				if (target != null && enemy != null)
				{
					float enemyDistanceToPacket = Vector3.Distance(enemy.transform.forward, target.transform.position);
					float distanceToPacket = Vector3.Distance(transform.forward, target.transform.position);

					if (distanceToPacket > enemyDistanceToPacket)
					{
						agentMemory.AmmoPacks.RemoveDetected(target);
						ExitAction(actionFailed);
					}
				}
			}
			yield return null;
		}
		
	}

	protected override void ExitAction(Action ExitAction)
	{
		if (Update != null)
		{
			StopCoroutine(Update);
		}

		IsActionDone = true;
		target = null;
		agentNavigation.InvalidateTarget();
		ExitAction?.Invoke();
	}
}
