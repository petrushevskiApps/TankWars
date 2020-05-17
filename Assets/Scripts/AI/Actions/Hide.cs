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

	private Coroutine RegenerateAction;

	public Hide() 
	{
		actionName = "Hide";

		AddPrecondition(StateKeys.ENEMY_DETECTED, false);
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
		AddListeners();
	}

	public override void ExecuteAction(GameObject agent)
	{
		Debug.Log($"<color=green> {gameObject.name} Perform Action: {actionName}</color>");
		RegenerateAction = StartCoroutine(Regenerate());
	}
	
	protected override void ExitAction(Action exitAction)
	{
		if(RegenerateAction != null)
		{
			StopCoroutine(RegenerateAction);
		}

		IsActionDone = true;
		target = null;
		agentNavigation.InvalidateTarget();
		RemoveListeners();
		exitAction?.Invoke();
	}

	IEnumerator Regenerate()
	{
		while(agent.GetInventory().HealthStatus != InventoryStatus.Full)
		{
			agent.GetInventory().IncreaseHealth(10);
			yield return new WaitForSeconds(1f);
		}
		while(agent.GetInventory().AmmoStatus != InventoryStatus.Full)
		{
			agent.GetInventory().IncreaseAmmo(2);
			yield return new WaitForSeconds(1f);
		}
		
		ExitAction(actionCompleted);
	}

	private void AddListeners()
	{
		agent.GetPerceptor().OnEnemyDetected.AddListener(OnEnemyDetected);
		agent.GetPerceptor().OnFriendlyDetected.AddListener(FriendlyUnityDetected);
	}
	private void RemoveListeners()
	{
		agent.GetPerceptor().OnEnemyDetected.RemoveListener(OnEnemyDetected);
		agent.GetPerceptor().OnFriendlyDetected.RemoveListener(FriendlyUnityDetected);
	}

	private void OnEnemyDetected(GameObject enemy)
	{
		if (target != null && enemy != null)
		{
			ExitAction(actionFailed);
		}
	}

	private void FriendlyUnityDetected(GameObject friend)
	{
		if (target != null && friend != null)
		{
			GoapAgent friendly = friend.GetComponent<GoapAgent>();

			if (friendly != null && friendly.GetCurrentAction().Equals("Hide"))
			{
				CompareDistanceToSpot(friend);
			}
		}
	}

	private void CompareDistanceToSpot(GameObject otherPlayer)
	{
		float otherDistanceToPacket = Vector3.Distance(otherPlayer.transform.position, target.transform.position);
		float distanceToPacket = Vector3.Distance(transform.position, target.transform.position);

		if (distanceToPacket > otherDistanceToPacket)
		{
			agentMemory.HidingSpots.RemoveDetected(target);
			ExitAction(actionFailed);
		}
	}
}
