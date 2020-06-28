using GOAP;
using System;
using System.Collections;
using UnityEngine;

public class FindAmmo : SearchAction 
{

	public FindAmmo()
	{
		AddPrecondition(StateKeys.AMMO_DETECTED, false);
		AddPrecondition(StateKeys.AMMO_FULL, false);

		AddEffect(StateKeys.AMMO_DETECTED, true);
	}

	// Searching for ammo refills cost should be affected
	// by the static search cost ( this is coast of uncertainty )
	// plus how many enemies are detected nearby and the agents
	// health inventory status, plus the agents ammo inventory status.
	public override float GetCost()
	{
		float TTR = (agent.Memory.IsUnderAttack ? 200 : 50) / agent.Navigation.currentSpeed;

		float ammoLimit = agent.Inventory.Ammo.GetInvertedCost();
		float healthLimit = agent.Inventory.Health.GetInvertedCost();

		float time = Mathf.Clamp((TTR - ammoLimit + healthLimit), 0, Mathf.Infinity);

		float cost = Mathf.Clamp(time * time, 1, Mathf.Infinity);
		return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
	}

	protected override void RegisterListeners()
	{
		base.RegisterListeners();
		agent.Sensors.OnUnderAttack.AddListener(AbortAction);

		agent.Memory.AmmoPacks.OnDetected.AddListener(AmmoDetected);
		agent.Memory.HealthPacks.OnDetected.AddListener(AbortAction);
		agent.Memory.HidingSpots.OnDetected.AddListener(AbortAction);

	}

	protected override void UnregisterListeners()
	{
		base.UnregisterListeners();
		agent.Sensors.OnUnderAttack.RemoveListener(AbortAction);

		agent.Memory.AmmoPacks.OnDetected.RemoveListener(AmmoDetected);
		agent.Memory.HealthPacks.OnDetected.RemoveListener(AbortAction);
		agent.Memory.HidingSpots.OnDetected.RemoveListener(AbortAction);
	}

	private void AmmoDetected()
	{
		ExitAction(actionCompleted);
	}

}
