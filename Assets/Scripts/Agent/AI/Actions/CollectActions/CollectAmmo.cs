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
		agent.Sensors.OnAmmoPackDetected.AddListener(AmmoDetected);
		agent.Sensors.OnHidingSpotDetected.AddListener(HidingSpotDetected);
	}
	protected override void RemoveListeners()
	{
		base.RemoveListeners();
		agent.Sensors.OnAmmoPackDetected.RemoveListener(AmmoDetected);
		agent.Sensors.OnHidingSpotDetected.RemoveListener(HidingSpotDetected);
	}

	private void AmmoDetected(GameObject ammoPack)
	{
		if(!ammoPack.Equals(target))
		{
			if (ammoPack != null && target != null)
			{
				if (Utilities.CompareDistances(transform.position, target.transform.position, ammoPack.transform.position) == 1)
				{
					SetActionTarget();
				}
			}
			
		}
		
	}
	private void HidingSpotDetected(GameObject hidingSpot)
	{
		if(hidingSpot != null && target != null)
		{
			if (Utilities.CompareDistances(transform.position, target.transform.position, hidingSpot.transform.position) == 1)
			{
				detectedMemory.InvalidateDetected(target);
				ExitAction(actionFailed);
			}
		}
	}

	
}
