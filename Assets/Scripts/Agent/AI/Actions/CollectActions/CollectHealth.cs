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
		agent.Memory.HealthPacks.OnDetected.AddListener(OnNewDetected);
		
	}
	protected override void RemoveListeners()
	{
		base.RemoveListeners();
		agent.Memory.HealthPacks.OnDetected.RemoveListener(OnNewDetected);
		
	}

	

	
}
