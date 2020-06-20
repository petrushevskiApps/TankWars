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

		AddPrecondition(StateKeys.HEALTH_DETECTED, true);

		AddEffect(StateKeys.HEALTH_FULL, true);

	}
	public override bool CheckProceduralPreconditions()
	{
		// Check if the agent is not under attack at
		// the moment of planning  and health is not full.
		return !agent.Memory.IsUnderAttack && !agent.Memory.IsHealthFull();
	}
	public override float GetCost()
	{
		float TTE = timeToExecute;
		float E = GetEnemyCost(agent.Memory.Enemies);
		float IH = GetInventoryCost(agent.Inventory.Health.Status, false);

		float cost = TTE + E - IH;
		return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
	}

	private new void Start()
	{
		base.Start();
		detectedMemory = agent.Memory.HealthPacks;
	}

	protected override IEnumerator CollectPickable()
	{
		yield return new WaitUntil(() => agent.Memory.IsHealthFull());
		ExitAction(actionCompleted);
	}

	//protected override void AddListeners()
	//{
	//	base.AddListeners();
	//	agent.Memory.HealthPacks.OnDetected.AddListener(OnNewDetected);
		
	//}
	//protected override void RemoveListeners()
	//{
	//	base.RemoveListeners();
	//	agent.Memory.HealthPacks.OnDetected.RemoveListener(OnNewDetected);
		
	//}

	

	
}
