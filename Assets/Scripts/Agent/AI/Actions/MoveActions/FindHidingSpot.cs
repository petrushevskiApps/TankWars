using GOAP;
using System;
using System.Collections;
using UnityEngine;

public class FindHidingSpot : SearchAction 
{

	public FindHidingSpot() 
	{
		AddPrecondition(StateKeys.HIDING_SPOT_DETECTED, false);

		AddEffect(StateKeys.HIDING_SPOT_DETECTED, true);
	}
	public override bool CheckProceduralPreconditions()
	{
		// Check if the agent is under attack at
		// the moment of planning.
		return !agent.Memory.IsUnderAttack && ( !agent.Memory.IsHealthFull() || !agent.Memory.IsAmmoFull() );
	}

	// Searching for health refills cost should be affected
	// by the static search cost ( this is coast of uncertantiy )
	// and how many enemies are detected nearby, minus the agents
	// health and ammo inventory status.
	public override float GetCost()
	{
		float IH = agent.Inventory.Health.GetCost();
		float IA = agent.Inventory.Ammo.GetCost();
		float E  = GetEnemyCost(agent.Memory.Enemies);

		float cost = searchCost + E - IH - IA;
		return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
	}

	protected override void AddListeners()
	{
		agent.Memory.HealthPacks.OnDetected.AddListener(HealthDetected);
		agent.Memory.HidingSpots.OnDetected.AddListener(HidingSpotDetected);
		agent.Sensors.OnUnderAttack.AddListener(UnderAttack);
	}
	
	protected override void RemoveListeners()
	{
		agent.Memory.HealthPacks.OnDetected.RemoveListener(HealthDetected);
		agent.Memory.HidingSpots.OnDetected.RemoveListener(HidingSpotDetected);
		agent.Sensors.OnUnderAttack.RemoveListener(UnderAttack);
	}

	private void UnderAttack(GameObject arg0)
	{
		ExitAction(actionFailed);
	}

	private void HealthDetected()
	{
		ExitAction(actionCompleted);
	}

	private void HidingSpotDetected()
	{
		ExitAction(actionCompleted);
	}

}
