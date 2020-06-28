using GOAP;
using System;
using System.Collections;
using UnityEngine;

public class FindHealth : SearchAction 
{

	public FindHealth() 
	{
		AddPrecondition(StateKeys.HEALTH_DETECTED, false);
		AddPrecondition(StateKeys.HEALTH_FULL, false);

		AddEffect(StateKeys.HEALTH_DETECTED, true);
	}
	

	// Searching for health refills cost should be affected
	// by the static search cost ( this is coast of uncertantiy )
	// and how many enemies are detected nearby, minus the agents
	// health inventory status.
	public override float GetCost()
	{

		float TTR = (agent.Memory.IsUnderAttack ? 200 : 50) / agent.Navigation.currentSpeed;
		float IH = agent.Inventory.Health.GetCost();

		float time = TTR - IH;
		float cost = Mathf.Clamp(time * time, 1, Mathf.Infinity);
		return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
	}

	protected override void AddListeners()
	{
		agent.Memory.Enemies.OnDetected.AddListener(EnemyDetected);
		agent.Memory.HealthPacks.OnDetected.AddListener(HealthDetected);
		agent.Memory.HidingSpots.OnDetected.AddListener(HidingSpotDetected);
		agent.Sensors.OnUnderAttack.AddListener(UnderAttack);
	}
	
	protected override void RemoveListeners()
	{
		agent.Memory.Enemies.OnDetected.RemoveListener(EnemyDetected);
		agent.Memory.HealthPacks.OnDetected.RemoveListener(HealthDetected);
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

	private void HidingSpotDetected()
	{
		ExitAction(actionFailed);
	}

	private void HealthDetected()
	{
		ExitAction(actionCompleted);
	}
}
