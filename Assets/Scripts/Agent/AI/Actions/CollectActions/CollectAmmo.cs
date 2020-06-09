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
		actionName = "CollectAmmo";

		AddPrecondition(StateKeys.AMMO_AVAILABLE, false);
		AddPrecondition(StateKeys.AMMO_DETECTED, true);
		AddPrecondition(StateKeys.UNDER_ATTACK, false);

		AddEffect(StateKeys.AMMO_AVAILABLE, true);

	}

	private new void Start()
	{
		base.Start();
		detectedMemory = agent.Memory.AmmoPacks;
	}

	protected override IEnumerator CollectPickable()
	{
		yield return new WaitUntil(() => agent.Memory.IsAmmoAvailable());
		ExitAction(actionCompleted);
	}

	protected override void AddListeners()
	{
		base.AddListeners();
		agent.Memory.AmmoPacks.OnDetected.AddListener(OnNewDetected);
	}
	protected override void RemoveListeners()
	{
		base.RemoveListeners();
		agent.Memory.AmmoPacks.OnDetected.RemoveListener(OnNewDetected);
	}

}
