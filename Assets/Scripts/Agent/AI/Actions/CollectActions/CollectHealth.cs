using System.Collections;
using UnityEngine;

public class CollectHealth : Collect 
{
	public CollectHealth() 
	{
		AddPrecondition(StateKeys.HEALTH_DETECTED, true);
		AddPrecondition(StateKeys.HEALTH_FULL, false);
		AddPrecondition(StateKeys.UNDER_ATTACK, false);

		AddEffect(StateKeys.HEALTH_FULL, true);

	}
	//public override bool CheckProceduralPreconditions()
	//{
	//	// Check if the agent is not under attack at
	//	// the moment of planning  and health is not full.
	//	return !agent.Memory.IsUnderAttack;
	//}
	public override float GetCost()
	{
		float TTE = timeToExecute;
		float TTR = TimeToReachCost(transform.position, agent.Memory.HealthPacks.GetSortedDetected(), agent.Navigation.currentSpeed);
		float E =  GetEnemyCost(agent.Memory.Enemies);
		float IH = agent.Inventory.Health.GetCost();

		float cost = TTE + TTR + E - ( IH * IH );
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
