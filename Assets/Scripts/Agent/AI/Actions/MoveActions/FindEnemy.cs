using GOAP;
using System;
using System.Collections;
using UnityEngine;

public class FindEnemy : SearchAction 
{
	public FindEnemy()
	{
		AddPrecondition(StateKeys.ENEMY_DETECTED, false);

		AddEffect(StateKeys.ENEMY_DETECTED, true);
	}

	// Searching for enemies cost should be determined
	// by agents inventory status of health and ammo
	public override float GetCost()
	{
		float IH = agent.Inventory.Health.GetCost();
		float IA = agent.Inventory.Ammo.GetCost();

		float cost = searchCost + (IH * IH) + IA;
		return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
	}

	protected override void AddListeners()
	{
		agent.Memory.Enemies.OnDetected.AddListener(EnemyDetected);
		agent.Sensors.OnUnderAttack.AddListener(UnderAttack);
	}
	
	protected override void RemoveListeners()
	{
		agent.Memory.Enemies.OnDetected.RemoveListener(EnemyDetected);
		agent.Sensors.OnUnderAttack.RemoveListener(UnderAttack);
	}

	private void EnemyDetected()
	{
		ExitAction(actionCompleted);
	}

	private void UnderAttack(GameObject arg0)
	{
		ExitAction(actionCompleted);
	}

}
