using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunAway : MoveAction 
{	
	public RunAway() 
	{
		actionName = "RunAway";

		AddPrecondition(StateKeys.ENEMY_DETECTED, true);
		AddPrecondition(StateKeys.AMMO_AMOUNT, false);

		AddEffect(StateKeys.ENEMY_DETECTED, false);
	}

	public override void SetActionTarget()
	{
		GameObject enemy = agentMemory.Enemies.GetDetected();
		agentNavigation.SetRunFromTarget(enemy);
		target = agentNavigation.GetNavigationTarget();
	}


	public override bool CheckPreconditions (GameObject agent)
	{	
		return true;
	}

}
