using GOAP;
using System;
using System.Collections;
using UnityEngine;

public class Patrol : MoveAction 
{

	public Patrol() 
	{
		actionName = "Patrol";

		AddPrecondition(StateKeys.PATROL, false);

		AddEffect(StateKeys.PATROL, true);
	}
	//public override bool CheckProceduralPreconditions()
	//{
	//	// Check if the agent is under attack at
	//	// the moment of planning.
	//	return !agent.Memory.IsUnderAttack;
	//}
	public override float GetCost()
	{
		float TTE = timeToExecute;
		float IH = GetInventoryCost(agent.Inventory.Health.Status, false);
		float IA = GetInventoryCost(agent.Inventory.Ammo.Status, false);
		float E = GetEnemyCost(agent.Memory.Enemies);

		float cost = TTE + IH + IA + E;
		return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
	}

	public override void SetActionTarget()
	{
		agent.Navigation.SetTarget();
		target = agent.Navigation.Target;
	}

	public override void InvalidTargetLocation()
	{
		SetActionTarget();
	}

	public override void EnterAction(Action Success, Action Fail, Action Reset)
	{
		base.EnterAction(Success, Fail, Reset);
	}

	public override void ExecuteAction(GameObject agent)
	{
		RestartAction();
	}

	protected override void ExitAction(Action ExitAction)
	{
		base.ExitAction(ExitAction);
	}

	protected override void AddListeners()
	{
		agent.Memory.Enemies.OnDetected.AddListener(EnemyDetected);
		agent.Memory.AmmoPacks.OnDetected.AddListener(AmmoDetected);
		agent.Memory.HealthPacks.OnDetected.AddListener(HealthDetected);
		agent.Memory.HidingSpots.OnDetected.AddListener(HidingSpotDetected);

		agent.Sensors.OnUnderAttack.AddListener(UnderAttack);
	}
	
	protected override void RemoveListeners()
	{
		agent.Memory.Enemies.OnDetected.RemoveListener(EnemyDetected);
		agent.Memory.AmmoPacks.OnDetected.RemoveListener(AmmoDetected);
		agent.Memory.HealthPacks.OnDetected.RemoveListener(HealthDetected);
		agent.Memory.HidingSpots.OnDetected.RemoveListener(HidingSpotDetected);

		agent.Sensors.OnUnderAttack.RemoveListener(UnderAttack);
	}

	private void EnemyDetected()
	{
		ExitAction(actionCompleted);
	}

	private void UnderAttack(GameObject arg0)
	{
		ExitAction(actionFailed);
	}

	private void HealthDetected()
	{
		if (!agent.Memory.IsHealthFull())
		{
			ExitAction(actionCompleted);
		}
	}

	private void AmmoDetected()
	{
		if (!agent.Memory.IsAmmoFull())
		{
			ExitAction(actionCompleted);
		}
	}
	
	private void HidingSpotDetected()
	{
		if (!agent.Memory.IsHealthFull() || !agent.Memory.IsAmmoFull())
		{
			ExitAction(actionCompleted);
		}
	}

	
}
