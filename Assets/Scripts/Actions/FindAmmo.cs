using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FindAmmo : GoapAction 
{
	private Vector3 destination;

	bool completed = false;

	private Memory agentMemory;

	public FindAmmo() 
	{
		name = "FindAmmo";

		AddPrecondition(StateKeys.AMMO_AMOUNT, false);
		AddPrecondition(StateKeys.AMMO_DETECTED, false);
		AddPrecondition(StateKeys.ENEMY_DETECTED, false);

		AddEffect(StateKeys.AMMO_DETECTED, true);

	}
	private void Start()
	{
		agentMemory = GetComponent<Tank>().agentMemory;
	}
	public override void Reset()
	{
		destination = transform.position;
		target = null;
		completed = false;
	}

	public override bool IsActionDone ()
	{
		return completed;
	}

	public override bool SetActionTarget()
	{
		target = agentMemory.Navigation.GetActionTarget();
		return target != null;
	}


	public override bool CheckProceduralPrecondition (GameObject agent)
	{
		if(agentMemory.AmmoPacks.IsAnyValidDetected())
		{
			target = agent;
		}

		return true;
	}

	public override void Perform(GameObject agent, Action success, Action fail)
	{
		if(agentMemory.AmmoPacks.IsAnyValidDetected())
		{
			success.Invoke();
			completed = true;
			return;
		}

		fail.Invoke();
	}

}
