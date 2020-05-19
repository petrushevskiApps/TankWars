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

		AddEffect(StateKeys.HEALTH_AMOUNT, true);

	}
	private new void Start()
	{
		base.Start();
		detectedMemory = agentMemory.HealthPacks;
	}

	protected override IEnumerator Collecting()
	{
		yield return new WaitUntil(() => agentMemory.IsHealthAvailable());
		ExitAction(actionCompleted);
	}
}
