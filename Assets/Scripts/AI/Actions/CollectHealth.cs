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
		yield return new WaitUntil(() => agentMemory.IsHealthAvailable());
		ExitAction(actionCompleted);
	}

	protected override void ExitAction(Action ExitAction)
	{
		IsActionDone = true;
		target = null;
		agentNavigation.InvalidateTarget();
		RemoveListeners();
		ExitAction?.Invoke();
	}


	private void AddListeners()
	{
		agent.GetPerceptor().OnEnemyDetected.AddListener(OnEnemyDetected);
		agent.GetPerceptor().OnHealthPackLost.AddListener(HealthPackLost);
		agent.GetPerceptor().OnFriendlyDetected.AddListener(FriendlyUnityDetected);
	}
	private void RemoveListeners()
	{
		agent.GetPerceptor().OnEnemyDetected.RemoveListener(OnEnemyDetected);
		agent.GetPerceptor().OnHealthPackLost.RemoveListener(HealthPackLost);
		agent.GetPerceptor().OnFriendlyDetected.RemoveListener(FriendlyUnityDetected);
	}

	private void HealthPackLost(GameObject healthPack)
	{
		if (healthPack.Equals(target))
		{
			ExitAction(actionFailed);
		}
	}


	private void OnEnemyDetected(GameObject enemy)
	{
		if (target != null && enemy != null)
		{
			CompareDistanceToPacket(enemy);
		}
	}

	private void FriendlyUnityDetected(GameObject friend)
	{
		if (target != null && friend != null)
		{
			GoapAgent friendly = friend.GetComponent<GoapAgent>();

			if (friendly != null && friendly.GetCurrentAction().Equals(actionName))
			{
				CompareDistanceToPacket(friend);
			}
		}
	}

	private void CompareDistanceToPacket(GameObject otherPlayer)
	{
		float otherDistanceToPacket = Vector3.Distance(otherPlayer.transform.position, target.transform.position);
		float distanceToPacket = Vector3.Distance(transform.position, target.transform.position);

		if (distanceToPacket > otherDistanceToPacket)
		{
			agentMemory.AmmoPacks.RemoveDetected(target);
			ExitAction(actionFailed);
		}
	}
}
