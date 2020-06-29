using GOAP;
using System;
using System.Collections;
using UnityEngine;

public class FindEnemy : SearchAction 
{
	public FindEnemy()
	{
		AddPrecondition(StateKeys.ENEMY_DETECTED, false);
		AddPrecondition(StateKeys.HEALTH_FULL, true);
		AddPrecondition(StateKeys.AMMO_FULL, true);

		AddEffect(StateKeys.ENEMY_DETECTED, true);
	}

	// For agent to search for enemies it should be
	// ready for fight.( health and ammo full ) So 
	// the cost for executing this action will be 1 always.
	public override float GetCost()
	{
		return 1;
	}

	protected override void RegisterListeners()
	{
		agent.Memory.Enemies.OnDetected.AddListener(EnemyDetected);
		agent.Sensors.OnUnderAttack.AddListener(UnderAttack);
	}

	protected override void UnregisterListeners()
	{
		agent.Memory.Enemies.OnDetected.RemoveListener(EnemyDetected);
		agent.Sensors.OnUnderAttack.RemoveListener(UnderAttack);
	}

	private void EnemyDetected()
	{
		ExitAction(actionCompleted, 0f);
	}

	private void UnderAttack(GameObject arg0)
	{
		ExitAction(actionCompleted, 0f);
	}

}
