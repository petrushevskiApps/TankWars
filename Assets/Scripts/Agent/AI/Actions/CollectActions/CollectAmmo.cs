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
		AddPrecondition(StateKeys.UNDER_ATTACK, false);

		AddEffect(StateKeys.AMMO_FULL, true);

	}
	//public override bool CheckProceduralPreconditions()
	//{
	//	// Check if the agent is not under attack at
	//	// the moment of planning and ammo is not full.
	//	return !agent.Memory.IsUnderAttack;
	//}
	public override float GetCost()
	{
		float TTE = timeToExecute;
		float TTR = TimeToReachCost(transform.position, agent.Memory.AmmoPacks.GetSortedDetected(), agent.Navigation.currentSpeed);
		float E  =	GetEnemyCost(agent.Memory.Enemies);
		float IA =  agent.Inventory.Ammo.GetCost();
		float IH =  agent.Inventory.Health.GetCost();

		float cost = TTE + TTR + E + (IH * IH) - IA;
		return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
	}

	private new void Start()
	{
		base.Start();
		detectedMemory = agent.Memory.AmmoPacks;
	}

	protected override IEnumerator CollectPickable()
	{
		yield return new WaitUntil(() => agent.Memory.IsAmmoFull());
		ExitAction(actionCompleted);
	}

	//protected override void AddListeners()
	//{
	//	base.AddListeners();
	//	agent.Memory.AmmoPacks.OnDetected.AddListener(OnNewDetected);
	//}
	//protected override void RemoveListeners()
	//{
	//	base.RemoveListeners();
	//	agent.Memory.AmmoPacks.OnDetected.RemoveListener(OnNewDetected);
	//}

}
