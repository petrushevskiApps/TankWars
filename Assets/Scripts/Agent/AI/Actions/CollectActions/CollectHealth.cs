using System;
using System.Collections;
using UnityEngine;

public class CollectHealth : Collect 
{
	public CollectHealth() 
	{
		AddPrecondition(StateKeys.HEALTH_DETECTED, true);
		AddPrecondition(StateKeys.HEALTH_FULL, false);

		AddEffect(StateKeys.HEALTH_FULL, true);

	}
	
	public override float GetCost()
	{
		GameObject healthPack = agent.Memory.HealthPacks.GetSortedDetected();
		float time = 0;

		if(healthPack != null)
		{
			time = TimeToReach(transform.position, healthPack, agent.Navigation.currentSpeed);
		}
		
		float healthLimit = agent.Inventory.Health.GetCost();

		float healthLimitedCost = Mathf.Clamp((time - healthLimit), 0, Mathf.Infinity);
		float cost = healthLimitedCost * healthLimitedCost;

		return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
	}

	private new void Start()
	{
		base.Start();
		detectedMemory = agent.Memory.HealthPacks;
	}

	protected override IEnumerator CollectPickable()
	{
		yield return new WaitUntil(() => agent.Memory.IsHealthFull());
		ExitAction(actionCompleted);
	}
}
