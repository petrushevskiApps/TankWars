using System;
using UnityEngine;

public class TacticalPosition : MoveAction 
{

	public TacticalPosition() 
	{
		actionName = "TacticalPosition";

		AddPrecondition(StateKeys.ENEMY_DETECTED, true);
		AddPrecondition(StateKeys.IN_SHOOTING_RANGE, false);
		AddPrecondition(StateKeys.AMMO_AMOUNT, true);
		AddPrecondition(StateKeys.HEALTH_AMOUNT, true);

		AddEffect(StateKeys.IN_SHOOTING_RANGE, true);
	}

	public override void SetActionTarget()
	{
		agentNavigation.SetTarget(CalculateTacticalPosition());
		target = agentNavigation.GetNavigationTarget();
	}


	public override bool CheckPreconditions (GameObject agent)
	{	
		return true;
	}

	private Vector3 CalculateTacticalPosition()
	{
		return gameObject.transform.position + (gameObject.transform.right * 5);
	}

}
