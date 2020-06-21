using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HideAndRegenerate : GoapAction 
{
	protected AIAgent agent;
	protected DetectedHolder detectedMemory;

	protected Coroutine UpdateCoroutine;

	private const float REGENERATE_INTERVAL = 1f;

	public HideAndRegenerate() 
	{
		AddPrecondition(StateKeys.HIDING_SPOT_DETECTED, true);

		AddEffect(StateKeys.HEALTH_FULL, true);
		AddEffect(StateKeys.AMMO_FULL, true);
	}

	public override bool CheckProceduralPreconditions()
	{
		// Check if the agent is not under attack at
		// the moment of planning 
		return !agent.Memory.IsUnderAttack;
	}

	public override float GetCost()
	{
		float TTE = timeToExecute;
		float TTR = TimeToReachCost(transform.position, agent.Memory.HidingSpots.GetSortedDetected(), agent.Navigation.currentSpeed);
		float E = GetEnemyCost(agent.Memory.Enemies);
		float IH = agent.Inventory.Health.GetCost();
		float IA = agent.Inventory.Ammo.GetCost();

		float cost = TTE + TTR + E - (IH * IH) - IA;
		return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
	}

	

	private void Start()
	{
		agent = GetComponent<AIAgent>();

		detectedMemory = agent.Memory.HidingSpots;
	}
	public override void SetActionTarget()
	{
		if (detectedMemory.IsAnyValidDetected())
		{
			target = detectedMemory.GetSortedDetected();
			agent.Navigation.SetTarget(target);
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

	

	public override void EnterAction(Action Success, Action Fail, Action Reset)
	{
		actionCompleted = Success;
		actionFailed = Fail;
		actionReset = Reset;

		SetActionTarget();

		AddListeners();
	}
	public override void ExecuteAction()
	{
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
			yield return new WaitForSeconds(REGENERATE_INTERVAL);
		}
		while(agent.Inventory.Ammo.Status != InventoryStatus.Full)
		{
			agent.Inventory.Ammo.Increase(4);
			yield return new WaitForSeconds(REGENERATE_INTERVAL);
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
		agent.Navigation.InvalidateTarget();

		ExitAction?.Invoke();
	}

	protected void AddListeners()
	{
		agent.Sensors.OnEnemyDetected.AddListener(OnAgentDetected);
		agent.Sensors.OnFriendlyDetected.AddListener(OnAgentDetected);
		agent.Sensors.OnUnderAttack.AddListener(OnUnderAttack);

		agent.Memory.HealthPacks.OnDetected.AddListener(HealthDetected);
		agent.Memory.AmmoPacks.OnDetected.AddListener(AmmoDetected);
		agent.Memory.HidingSpots.OnDetected.AddListener(HidingSpotDetected);
	}
	protected void RemoveListeners()
	{
		agent.Sensors.OnEnemyDetected.RemoveListener(OnAgentDetected);
		agent.Sensors.OnFriendlyDetected.RemoveListener(OnAgentDetected);
		agent.Sensors.OnUnderAttack.RemoveListener(OnUnderAttack);

		agent.Memory.HealthPacks.OnDetected.RemoveListener(HealthDetected);
		agent.Memory.AmmoPacks.OnDetected.RemoveListener(AmmoDetected);
		agent.Memory.HidingSpots.OnDetected.RemoveListener(HidingSpotDetected);
	}

	// When attacked abort current action and
	// re-plan accordingly
	private void OnUnderAttack(GameObject arg0)
	{
		if(target != null)
		{
			detectedMemory.InvalidateDetected(target);
		}
		ExitAction(actionFailed);
	}

	// On agent detected, check if the agent is
	// executing same action. If it does, check
	// is it closer to target and abort and re-plan
	// otherwise continue
	private void OnAgentDetected(GameObject agent)
	{
		if (target != null && agent != null)
		{
			GoapAgent gaOther = agent.GetComponent<GoapAgent>();

			if (gaOther != null && gaOther.GetCurrentAction().Equals(ActionName))
			{
				CompareDistanceToPacket(agent);
			}
		}
	}
	private void CompareDistanceToPacket(GameObject otherPlayer)
	{
		float otherDistanceToPacket = GetDistanceToCollectible(otherPlayer);
		float distanceToPacket = GetDistanceToCollectible(gameObject);

		if (distanceToPacket > otherDistanceToPacket)
		{
			if (distanceToPacket < 20)
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


	// On new hiding spot detected
	// resetting target will re-sort detected
	// spots and update target to the closest one
	private void HidingSpotDetected()
	{
		SetActionTarget();
	}

	// On new ammo pickable detected
	// if health is available abort action
	// and re-plan accordingly
	private void AmmoDetected()
	{
		if (agent.Memory.IsHealthFull() && target != null)
		{
			detectedMemory.InvalidateDetected(target);
			ExitAction(actionFailed);
		}
	}

	// On new health pickable detected
	// if ammo is available abort action
	// and re-plan accordingly
	private void HealthDetected()
	{
		if (agent.Memory.IsAmmoFull() && target != null)
		{
			detectedMemory.InvalidateDetected(target);
			ExitAction(actionFailed);
		}
	}

	
}
