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

		AddEffect(StateKeys.AMMO_DETECTED, true);

	}
	private void Awake()
	{
		agentMemory = GetComponent<Tank>().agentMemory;
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
		return  true;
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
				if (agentMemory.AmmoPacks.IsAnyDetected())
				{
					succes.Invoke();
					navAgent.isStopped = true;
					navAgent.isStopped = false;
					completed = true;
					break;
				}
				else if (navAgent.remainingDistance <= maxRequiredRange)
				{
					fail.Invoke();
					break;
				}

				yield return new WaitForSeconds(0.3f);
			}
		}
		else
		{
			fail.Invoke();
		}
	}

	
}
