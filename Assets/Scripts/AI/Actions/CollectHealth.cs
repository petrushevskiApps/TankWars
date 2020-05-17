using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollectHealth : GoapAction 
{
	private IGoap agent;
	private MemorySystem agentMemory;
	private NavigationSystem agentNavigation;

	
	public CollectHealth() 
	{
		actionName = "CollectHealth";

		AddPrecondition(StateKeys.HEALTH_AMOUNT, false);
		AddPrecondition(StateKeys.HEALTH_DETECTED, true);

		AddEffect(StateKeys.HEALTH_AMOUNT, true);

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
		if (agentMemory.HealthPacks.IsAnyValidDetected())
		{
			target = agentMemory.HealthPacks.GetDetected();
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

		AddListeners();
	}

	public override void ExecuteAction(GameObject agent)
	{
		Debug.Log($"<color=green> {gameObject.name} Perform Action: {actionName}</color>");

		if (target != null && target.activeSelf)
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
		yield return new WaitUntil(() => this.agent.GetInventory().IsHealthAvailable());
		agentMemory.HealthPacks.RemoveDetected(target);
		ExitAction(actionCompleted);
	}

	protected override void ExitAction(Action exitAction)
	{
		IsActionDone = true;
		target = null;
		agentNavigation.InvalidateTarget();
		RemoveListeners();
		exitAction?.Invoke();
	}


	private void AddListeners()
	{
		agent.GetPerceptor().OnEnemyDetected.AddListener(OnEnemyDetected);
	}
	private void RemoveListeners()
	{
		agent.GetPerceptor().OnEnemyDetected.AddListener(OnEnemyDetected);
	}

	private void OnEnemyDetected(GameObject enemy)
	{
		if (target != null && enemy != null)
		{
			float enemyDistanceToPacket = Vector3.Distance(enemy.transform.position, target.transform.position);
			float distanceToPacket = Vector3.Distance(transform.position, target.transform.position);

			if (distanceToPacket > enemyDistanceToPacket)
			{
				ExitAction(actionFailed);
			}
		}
	}
}
