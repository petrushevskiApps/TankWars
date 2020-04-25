using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : GoapAction 
{
	private Vector3 destination;
	private Coroutine actionCoroutine;

	bool completed = false;

	public Patrol() 
	{
		AddPrecondition(StateKeys.ENEMY_DETECTED, false);
		AddPrecondition(StateKeys.AMMO_AMOUNT, true);

		AddEffect(GoalKeys.PATROL, true);
		name = "Patrol";
	}
	
	public override void Reset ()
	{
		destination = transform.position;
		completed = false;
	}
	
	public override bool IsActionDone ()
	{
		return completed;
	}
	
	public override bool RequiresInRange ()
	{
		return false; 
	}
	
	public override bool CheckProceduralPrecondition (GameObject agent)
	{	
		return GetComponent<Tank>().agentMemory.Enemies.IsAnyDetected() == false;
	}
	
	public override void Perform(GameObject agent, Action success, Action fail)
	{
		Debug.Log($"<color=green> {gameObject.name} Perform Action: {this.name}</color>");
		StartCoroutine(MoveAgent(agent, success, fail));
	}

	IEnumerator MoveAgent(GameObject agent, Action succes, Action fail)
	{
		destination = CornerCalculator.Instance.GetRandomInWorldCoordinates();
		NavMeshAgent navAgent = agent.GetComponent<NavMeshAgent>();

		navAgent.isStopped = false;
		NavMeshPath path = new NavMeshPath();
		navAgent.CalculatePath(destination, path);
		navAgent.stoppingDistance = maxRequiredRange;

		if(path.status == NavMeshPathStatus.PathComplete)
		{
			navAgent.SetPath(path);

			while(true)
			{
				if(CheckProceduralPrecondition(agent))
				{
					if (navAgent.remainingDistance <= maxRequiredRange)
					{
						succes.Invoke();
						completed = true;
						break;
					}
				}
				else
				{
					fail.Invoke();
					break;
				}
				

				yield return null;
			}
		}
		else
		{
			fail.Invoke();
		}
	}

	
}
