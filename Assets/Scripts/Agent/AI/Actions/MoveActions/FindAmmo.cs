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
		AddPrecondition(StateKeys.UNDER_ATTACK, false);

		AddEffect(StateKeys.AMMO_DETECTED, true);
	}
	//public override bool CheckProceduralPreconditions()
	//{
	//	// Check if the agent is under attack at
	//	// the moment of planning.
	//	return !agent.Memory.IsUnderAttack;
	//}

	// Searching for ammo refills cost should be affected
	// by the static search cost ( this is coast of uncertainty )
	// plus how many enemies are detected nearby and the agents
	// health inventory status, plus the agents ammo inventory status.
	public override float GetCost()
	{
		float IH = agent.Inventory.Health.GetCost();
		float IA = agent.Inventory.Ammo.GetCost();
		float E = GetEnemyCost(agent.Memory.Enemies);

		float cost = searchCost + E + IH - (IA * IA);
		return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
	}

	protected override void AddListeners()
	{
		agent.Memory.AmmoPacks.OnDetected.AddListener(AmmoDetected);
		agent.Memory.HidingSpots.OnDetected.AddListener(HidingSpotDetected);

		agent.Sensors.OnUnderAttack.AddListener(UnderAttack);
	}
	
	protected override void RemoveListeners()
	{
		agent.Memory.AmmoPacks.OnDetected.RemoveListener(AmmoDetected);
		agent.Memory.HidingSpots.OnDetected.RemoveListener(HidingSpotDetected);

		agent.Sensors.OnUnderAttack.RemoveListener(UnderAttack);
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
