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

	protected override void RegisterListeners()
	{
		base.RegisterListeners();
		agent.Sensors.OnUnderAttack.AddListener(ActionAbort);

		agent.Memory.HealthPacks.OnDetected.AddListener(HealthDetected);
		agent.Memory.AmmoPacks.OnDetected.AddListener(ReplanningAbort);
		agent.Memory.HidingSpots.OnDetected.AddListener(ReplanningAbort);

	}
	protected override void UnregisterListeners()
	{
		base.UnregisterListeners();
		agent.Sensors.OnUnderAttack.RemoveListener(ActionAbort);

		agent.Memory.HealthPacks.OnDetected.RemoveListener(HealthDetected);
		agent.Memory.AmmoPacks.OnDetected.RemoveListener(ReplanningAbort);
		agent.Memory.HidingSpots.OnDetected.RemoveListener(ReplanningAbort);
	}

	private void HealthDetected()
	{
		ExitAction(actionCompleted, 0f);
	}
}
