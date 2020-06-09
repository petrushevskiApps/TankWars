using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

public class CollectHealth : Collect 
{
	public CollectHealth() 
	{
		actionName = "CollectHealth";

		AddPrecondition(StateKeys.HEALTH_AMOUNT, false);
		AddPrecondition(StateKeys.HEALTH_DETECTED, true);
		AddPrecondition(StateKeys.UNDER_ATTACK, false);

		AddEffect(StateKeys.HEALTH_AMOUNT, true);

	}
	private new void Start()
	{
		base.Start();
		detectedMemory = agent.Memory.HealthPacks;
	}
	protected override IEnumerator CollectPickable()
	{
		yield return new WaitUntil(() => agent.Memory.IsHealthAvailable());
		ExitAction(actionCompleted);
	}

	protected override void AddListeners()
	{
		base.AddListeners();
		agent.Sensors.OnHealthPackDetected.AddListener(HealthDetected);
		agent.Sensors.OnHidingSpotDetected.AddListener(HidingSpotDetected);
	}
	protected override void RemoveListeners()
	{
		base.RemoveListeners();
		agent.Sensors.OnHealthPackDetected.RemoveListener(HealthDetected);
		agent.Sensors.OnHidingSpotDetected.RemoveListener(HidingSpotDetected);
	}

	private void HealthDetected(GameObject healthPack)
	{
		if (!healthPack.Equals(target))
		{
			if (Utilities.CompareDistances(transform.position, target.transform.position, healthPack.transform.position) == 1)
			{
				SetActionTarget();
			}
		}

	}
	private void HidingSpotDetected(GameObject hidingSpot)
	{
		if(target != null && hidingSpot != null)
		{
			if (Utilities.CompareDistances(transform.position, target.transform.position, hidingSpot.transform.position) == 1)
			{
				detectedMemory.InvalidateDetected(target);
				ExitAction(actionFailed);
			}
		}
	}
}
