using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollectAmmo : Collect 
{
	public CollectAmmo() 
	{
		AddPrecondition(StateKeys.AMMO_DETECTED, true);
		AddPrecondition(StateKeys.AMMO_FULL, false);

		AddEffect(StateKeys.AMMO_FULL, true);
	}
	private void Start()
	{
		detectedMemory = agent.Memory.AmmoPacks;
	}

	public override float GetCost()
	{
		GameObject ammoPack = agent.Memory.AmmoPacks.GetSortedDetected();
		float time = 0;

		if (ammoPack != null)
		{
			time = Utilities.TimeToReach(transform.position, ammoPack, agent.Navigation.currentSpeed);
		}
		
		float ammoLimit = agent.Inventory.Ammo.GetInvertedCost();
		float healthLimit = agent.Inventory.Health.GetInvertedCost();

		float ammoLimitedCost = Mathf.Clamp((time - ammoLimit + healthLimit), 0, Mathf.Infinity);
		float cost = (ammoLimitedCost * ammoLimitedCost) + 2;

		return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
	}

	protected override IEnumerator CollectPickable()
	{
		yield return new WaitUntil(() => agent.Inventory.Ammo.IsFull || (target == null));
		ExitAction(actionCompleted, 0f);
	}
}
