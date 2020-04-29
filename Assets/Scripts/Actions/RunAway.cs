using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunAway : GoapAction 
{

	bool completed = false;
	private Memory agentMemory;

	public RunAway() 
	{
		name = "RunAway";

		AddPrecondition(StateKeys.ENEMY_DETECTED, true);
		AddPrecondition(StateKeys.AMMO_AMOUNT, false);


		AddEffect(StateKeys.ENEMY_DETECTED, false);
	}
	private void Start()
	{
		agentMemory = GetComponent<Tank>().agentMemory;
	}
	public override void Reset ()
	{
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
		return true;
	}
	
	public override void Perform(GameObject agent, Action success, Action fail)
	{
		if(!agentMemory.Enemies.IsAnyValidDetected())
		{
			success.Invoke();
			completed = true;
		}
		else
		{
			fail.Invoke();
		}
	}


}
