using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HideAndRegenerate : Collect 
{
	public HideAndRegenerate() 
	{
		actionName = "HideAndRegenerate";

		AddPrecondition(StateKeys.ENEMY_DETECTED, false);
		AddPrecondition(StateKeys.HEALTH_AMOUNT, false);
		AddPrecondition(StateKeys.AMMO_AMOUNT, false);
		AddPrecondition(StateKeys.HIDING_SPOT_DETECTED, true);

		AddEffect(StateKeys.HEALTH_AMOUNT, true);
		AddEffect(StateKeys.AMMO_AMOUNT, true);
	}
	private new void Start()
	{
		base.Start();
		detectedMemory = agentMemory.HidingSpots;
	}


	protected override IEnumerator ActionUpdate()
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
}
