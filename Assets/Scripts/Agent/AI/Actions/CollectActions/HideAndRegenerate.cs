using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HideAndRegenerate : GoapAction 
{
	protected AIAgent agent;
	protected MemoryController agentMemory;
	protected NavigationController agentNavigation;
	protected DetectedHolder detectedMemory;

	protected Coroutine UpdateCoroutine;

	public HideAndRegenerate() 
	{
		actionName = "HideAndRegenerate";

		AddPrecondition(StateKeys.UNDER_ATTACK, false);
		AddPrecondition(StateKeys.HIDING_SPOT_DETECTED, true);

		AddEffect(StateKeys.HEALTH_AMOUNT, true);
		AddEffect(StateKeys.AMMO_AVAILABLE, true);
	}
	private void Start()
	{
		agent = GetComponent<AIAgent>();
		agentMemory = agent.Memory;
		agentNavigation = agent.Navigation;

		detectedMemory = agentMemory.HidingSpots;
	}
	public override void SetActionTarget()
	{
		if (detectedMemory.IsAnyValidDetected())
		{
			target = detectedMemory.GetSortedDetected();
			agentNavigation.SetTarget(target);
		}
		else
		{
			ExitAction(actionFailed);
		}
	}
	public override void InvalidTargetLocation()
	{
		detectedMemory.InvalidateDetected(target);
		SetActionTarget();
	}

	protected bool IsTargetValid()
	{
		return target != null && target.activeSelf;
	}

	public override bool TestProceduralPreconditions()
	{
		return !agentMemory.IsHealthAvailable() || !agentMemory.IsAmmoAvailable();
	}
	public bool CheckActionConditions()
	{
		return IsTargetValid();
	}
	public override void EnterAction(Action Success, Action Fail, Action Reset)
	{
		actionCompleted = Success;
		actionFailed = Fail;
		actionReset = Reset;

		SetActionTarget();

		AddListeners();
	}
	public override void ExecuteAction(GameObject agent)
	{
		Debug.Log($"<color=green> {gameObject.name} Perform Action: {actionName}</color>");

		if (!detectedMemory.IsDetectedValid(target))
		{
			ExitAction(actionFailed);
		}
		else
		{
			UpdateCoroutine = StartCoroutine(ActionUpdate());
		}
	}
	protected IEnumerator ActionUpdate()
	{
		if (!detectedMemory.IsDetectedValid(target)) ExitAction(actionFailed);

		while(agent.Inventory.Health.Status != InventoryStatus.Full)
		{
			agent.Inventory.Health.Increase(10);
			yield return new WaitForSeconds(1f);
		}
		while(agent.Inventory.Ammo.Status != InventoryStatus.Full)
		{
			agent.Inventory.Ammo.Increase(2);
			yield return new WaitForSeconds(1f);
		}
		
		ExitAction(actionCompleted);
	}

	protected override void ExitAction(Action ExitAction)
	{
		RemoveListeners();
		IsActionDone = true;

		if (UpdateCoroutine != null)
		{
			StopCoroutine(UpdateCoroutine);
			UpdateCoroutine = null;
		}
		target = null;
		agentNavigation.InvalidateTarget();

		ExitAction?.Invoke();
	}
	protected void AddListeners()
	{
		agent.GetPerceptor().OnEnemyDetected.AddListener(OnOtherDetected);
		agent.GetPerceptor().OnFriendlyDetected.AddListener(OnOtherDetected);
		agent.GetPerceptor().OnUnderAttack.AddListener(OnUnderAttack);

		agent.GetPerceptor().OnHealthPackDetected.AddListener(HealthDetected);
		agent.GetPerceptor().OnAmmoPackDetected.AddListener(AmmoDetected);
		agent.GetPerceptor().OnHidingSpotDetected.AddListener(HidingSpotDetected);
	}
	protected void RemoveListeners()
	{
		agent.GetPerceptor().OnEnemyDetected.RemoveListener(OnOtherDetected);
		agent.GetPerceptor().OnFriendlyDetected.RemoveListener(OnOtherDetected);
		agent.GetPerceptor().OnUnderAttack.RemoveListener(OnUnderAttack);

		agent.GetPerceptor().OnHealthPackDetected.RemoveListener(HealthDetected);
		agent.GetPerceptor().OnAmmoPackDetected.RemoveListener(AmmoDetected);
		agent.GetPerceptor().OnHidingSpotDetected.RemoveListener(HidingSpotDetected);
	}

	private void OnUnderAttack(GameObject arg0)
	{
		if(target != null)
		{
			detectedMemory.InvalidateDetected(target);
		}
		ExitAction(actionFailed);
	}

	private void OnOtherDetected(GameObject other)
	{
		if (target != null && other != null)
		{
			GoapAgent gaOther = other.GetComponent<GoapAgent>();

			if (gaOther != null && gaOther.GetCurrentAction().Equals(actionName))
			{
				CompareDistanceToPacket(other);
			}
		}
	}
	private void CompareDistanceToPacket(GameObject otherPlayer)
	{
		float otherDistanceToPacket = GetDistanceToCollectible(otherPlayer);
		float distanceToPacket = GetDistanceToCollectible(gameObject);

		if (distanceToPacket > otherDistanceToPacket)
		{
			if (distanceToPacket < 21)
			{
				detectedMemory.InvalidateDetected(target);
				ExitAction(actionFailed);
			}
		}
	}

	public float GetDistanceToCollectible(GameObject player)
	{
		if (player != null && target != null)
		{
			return Vector3.Distance(player.transform.position, target.transform.position);
		}
		else
		{
			return Mathf.Infinity;
		}
	}

	

	private void HidingSpotDetected(GameObject detected)
	{
		if (!detected.Equals(target))
		{
			if(target != null && detected != null)
			{
				if (Utilities.CompareDistances(transform.position, target.transform.position, detected.transform.position) == 1)
				{
					SetActionTarget();
				}
			}
			
		}
	}

	private void AmmoDetected(GameObject detected)
	{
		if (agentMemory.IsHealthAvailable())
		{
			if (target != null && detected != null)
			{
				if (Utilities.CompareDistances(transform.position, target.transform.position, detected.transform.position) == 1)
				{
					detectedMemory.InvalidateDetected(target);
					ExitAction(actionFailed);
				}
			}
				
		}
	}

	private void HealthDetected(GameObject detected)
	{
		if(agentMemory.IsAmmoAvailable())
		{
			if(target != null && detected != null)
			{
				if (Utilities.CompareDistances(transform.position, target.transform.position, detected.transform.position) == 1)
				{
					detectedMemory.InvalidateDetected(target);
					ExitAction(actionFailed);
				}
			}
			
		}
	}

	
}
