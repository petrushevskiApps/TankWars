using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollectAmmo : Collect 
{
	public CollectAmmo() 
	{
		actionName = "CollectAmmo";

		AddPrecondition(StateKeys.AMMO_AMOUNT, false);
		AddPrecondition(StateKeys.AMMO_DETECTED, true);

		AddEffect(StateKeys.AMMO_AMOUNT, true);

	}
	private new void Start()
	{
		base.Start();
		detectedMemory = agentMemory.AmmoPacks;
	}

	protected override IEnumerator ActionUpdate()
	{
		yield return new WaitUntil(() => agentMemory.IsAmmoAvailable());
		ExitAction(actionCompleted);
	}
}
