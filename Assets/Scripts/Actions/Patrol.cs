using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : GoapAction 
{

	private bool completed = false;

	private Memory agentMemory;

	public Patrol() 
	{
		name = "Patrol";

		AddPrecondition(StateKeys.ENEMY_DETECTED, false);
		AddPrecondition(StateKeys.AMMO_AMOUNT, true);

		AddEffect(GoalKeys.PATROL, true);
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
		return !agentMemory.Enemies.IsAnyValidDetected();
	}
	
	public override void Perform(GameObject agent, Action success, Action fail)
	{
		success.Invoke();
		completed = true;
	}



}
