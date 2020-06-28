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

	protected override void AddListeners()
	{
		agent.Memory.Enemies.OnDetected.AddListener(EnemyDetected);
		agent.Memory.AmmoPacks.OnDetected.AddListener(AmmoDetected);
		agent.Memory.HidingSpots.OnDetected.AddListener(HidingSpotDetected);

		agent.Sensors.OnUnderAttack.AddListener(UnderAttack);
	}
	
	protected override void RemoveListeners()
	{
		agent.Memory.Enemies.OnDetected.RemoveListener(EnemyDetected);
		agent.Memory.AmmoPacks.OnDetected.RemoveListener(AmmoDetected);
		agent.Memory.HidingSpots.OnDetected.RemoveListener(HidingSpotDetected);

		agent.Sensors.OnUnderAttack.RemoveListener(UnderAttack);
	}
	private void EnemyDetected()
	{
		ExitAction(actionFailed);
	}

	private void UnderAttack(GameObject arg0)
	{
		ExitAction(actionFailed);
	}

	private void AmmoDetected()
	{
		ExitAction(actionCompleted);
	}
	
	private void HidingSpotDetected()
	{
		ExitAction(actionFailed);
	}

}
