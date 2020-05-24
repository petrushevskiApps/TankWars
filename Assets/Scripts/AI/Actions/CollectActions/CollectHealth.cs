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
		detectedMemory = agentMemory.HealthPacks;
	}

	protected override IEnumerator ActionUpdate()
	{
		if (!isReady) ExitAction(actionFailed);

		yield return new WaitUntil(() => agentMemory.IsHealthAvailable());
		ExitAction(actionCompleted);
	}

	

	protected override void AddListeners()
	{
		base.AddListeners();
		agent.GetPerceptor().OnHealthPackDetected.AddListener(HealthDetected);
		agent.GetPerceptor().OnHidingSpotDetected.AddListener(HidingSpotDetected);
	}
	protected override void RemoveListeners()
	{
		base.RemoveListeners();
		agent.GetPerceptor().OnHealthPackDetected.RemoveListener(HealthDetected);
		agent.GetPerceptor().OnHidingSpotDetected.RemoveListener(HidingSpotDetected);
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
		if (Utilities.CompareDistances(transform.position, target.transform.position, hidingSpot.transform.position) == 1)
		{
			Invalidate();
			ExitAction(actionFailed);
		}
	}
}
