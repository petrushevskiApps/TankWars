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
		detectedMemory = agentMemory.AmmoPacks;
	}

	protected override IEnumerator ActionUpdate()
	{
		if(!isReady) ExitAction(actionFailed);

		yield return new WaitUntil(agentMemory.IsAmmoAvailable);
		ExitAction(actionCompleted);
	}

	protected override void AddListeners()
	{
		base.AddListeners();
		agent.GetPerceptor().OnAmmoPackDetected.AddListener(AmmoDetected);
		agent.GetPerceptor().OnHidingSpotDetected.AddListener(HidingSpotDetected);
	}
	protected override void RemoveListeners()
	{
		base.RemoveListeners();
		agent.GetPerceptor().OnAmmoPackDetected.RemoveListener(AmmoDetected);
		agent.GetPerceptor().OnHidingSpotDetected.RemoveListener(HidingSpotDetected);
	}

	private void AmmoDetected(GameObject ammoPack)
	{
		if(!ammoPack.Equals(target))
		{
			if(Utilities.CompareDistances(transform.position, target.transform.position, ammoPack.transform.position) == 1)
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
