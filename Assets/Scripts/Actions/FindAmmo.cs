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

	public override void SetActionTarget()
	{
		agentMemory.Navigation.SetTarget();
		target = agentMemory.Navigation.GetTarget();
	}

	public override bool CheckProceduralPrecondition (GameObject agent)
	{
		if(agentMemory.AmmoPacks.IsAnyValidDetected())
		{
			agentMemory.Navigation.AbortMoving();
		}

		return true;
	}

	public override void Perform(GameObject agent, Action success, Action fail)
	{
		Debug.Log($"<color=green> {agent.name} Perform Action: {name}</color>");

		if (agentMemory.AmmoPacks.IsAnyValidDetected())
		{
			success.Invoke();
			completed = true;
			return;
		}

		fail.Invoke();
	}

	
}
