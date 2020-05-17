using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollectAmmo : GoapAction 
{

	private IGoap agent;
	private MemorySystem agentMemory;
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

		AddListeners();
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


	protected override void ExitAction(Action ExitAction)
	{
		RemoveListeners();
		IsActionDone = true;
		target = null;
		agentNavigation.InvalidateTarget();
		ExitAction?.Invoke();
	}

	private void AddListeners()
	{
		agent.GetPerceptor().OnEnemyDetected.AddListener(OnEnemyDetected);
		agent.GetPerceptor().OnAmmoPackLost.AddListener(AmmoPackLost);
		agent.GetPerceptor().OnFriendlyDetected.AddListener(FriendlyUnityDetected);
	}

	

	private void RemoveListeners()
	{
		agent.GetPerceptor().OnEnemyDetected.RemoveListener(OnEnemyDetected);
		agent.GetPerceptor().OnAmmoPackLost.RemoveListener(AmmoPackLost);
		agent.GetPerceptor().OnFriendlyDetected.RemoveListener(FriendlyUnityDetected);
	}

	private void AmmoPackLost(GameObject ammoPack)
	{
		if(ammoPack.Equals(target))
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
		if(target != null && friend != null)
		{
			GoapAgent friendly = friend.GetComponent<GoapAgent>();
			
			if (friendly != null && friendly.GetCurrentAction().Equals("CollectAmmo"))
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
