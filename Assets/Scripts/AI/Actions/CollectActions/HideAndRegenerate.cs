using GOAP;
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

		AddPrecondition(StateKeys.UNDER_ATTACK, false);
		AddPrecondition(StateKeys.HIDING_SPOT_DETECTED, true);

		AddEffect(StateKeys.HEALTH_AMOUNT, true);
		AddEffect(StateKeys.AMMO_AVAILABLE, true);
	}
	private new void Start()
	{
		base.Start();
		detectedMemory = agentMemory.HidingSpots;
	}

	public override bool TestProceduralPreconditions()
	{
		return !agentMemory.IsHealthAvailable() || !agentMemory.IsAmmoAvailable();
	}

	protected override IEnumerator ActionUpdate()
	{
		if (!isReady) ExitAction(actionFailed);

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

	protected override void AddListeners()
	{
		base.AddListeners();
		agent.GetPerceptor().OnHealthPackDetected.AddListener(HealthDetected);
		agent.GetPerceptor().OnAmmoPackDetected.AddListener(AmmoDetected);
		agent.GetPerceptor().OnHidingSpotDetected.AddListener(HidingSpotDetected);
	}
	protected override void RemoveListeners()
	{
		base.RemoveListeners();
		agent.GetPerceptor().OnHealthPackDetected.RemoveListener(HealthDetected);
		agent.GetPerceptor().OnAmmoPackDetected.RemoveListener(AmmoDetected);
		agent.GetPerceptor().OnHidingSpotDetected.RemoveListener(HidingSpotDetected);
	}

	private void HidingSpotDetected(GameObject detected)
	{
		if (!detected.Equals(target))
		{
			if(target != null && detected != null)
			{
				if (Utilities.CompareDistances(transform.position, target.transform.position, detected.transform.position) == 1)
				{
					SetActionTarget();
				}
			}
			
		}
	}

	private void AmmoDetected(GameObject detected)
	{
		if (agentMemory.IsHealthAvailable())
		{
			if (target != null && detected != null)
			{
				if (Utilities.CompareDistances(transform.position, target.transform.position, detected.transform.position) == 1)
				{
					Invalidate();
					ExitAction(actionFailed);
				}
			}
				
		}
	}

	private void HealthDetected(GameObject detected)
	{
		if(agentMemory.IsAmmoAvailable())
		{
			if(target != null && detected != null)
			{
				if (Utilities.CompareDistances(transform.position, target.transform.position, detected.transform.position) == 1)
				{
					Invalidate();
					ExitAction(actionFailed);
				}
			}
			
		}
	}

	
}
