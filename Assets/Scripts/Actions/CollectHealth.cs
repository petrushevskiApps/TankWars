using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollectHealth : GoapAction 
{

	private IGoap agent;
	private Memory agentMemory;
	private NavigationSystem agentNavigation;

	private bool healthCollected = false;
	private bool completed = false;

	public CollectHealth() 
	{
		name = "CollectHealth";

		AddPrecondition(StateKeys.HEALTH_AMOUNT, false);
		AddPrecondition(StateKeys.HEALTH_DETECTED, true);

		AddEffect(StateKeys.HEALTH_AMOUNT, true);

	}
	private void Start()
	{
		agent = GetComponent<IGoap>();
		agentMemory = agent.GetMemory();
		agentNavigation = agent.GetNavigation();
	}
	
	public override void Reset ()
	{
		target = null;
		healthCollected = false;
		completed = false;
	}
	
	public override bool IsActionDone ()
	{
		return completed;
	}

	public override void SetActionTarget()
	{
		if (agentMemory.HealthPacks.IsAnyValidDetected())
		{
			target = agentMemory.HealthPacks.GetDetected();
			agentNavigation.SetTarget(target);
		}
	}
	

	public override bool CheckPreconditions (GameObject agentGo)
	{
		if (agentMemory.Enemies.IsAnyValidDetected())
		{
			GameObject enemy = agentMemory.Enemies.GetDetected();
			
			if(target != null && enemy != null)
			{
				float enemyDistanceToPacket = Vector3.Distance(enemy.transform.position, target.transform.position);
				float distanceToPacket = Vector3.Distance(agentGo.transform.position, target.transform.position);

				if (distanceToPacket > enemyDistanceToPacket)
				{
					agentMemory.HealthPacks.InvalidateDetected(target);
					agentNavigation.InvalidateTarget();
					return false;
				}
			}
		}

		return true;
	}
	
	public override void ExecuteAction(GameObject agent, Action success, Action fail)
	{
		Debug.Log($"<color=green> {gameObject.name} Perform Action: {name}</color>");
		StartCoroutine(Collect(success, fail));
	}
	
	protected override void ExitAction(Action exitAction)
	{
		agentNavigation.InvalidateTarget();
		exitAction?.Invoke();
	}

	IEnumerator Collect(Action succes, Action fail)
	{
		yield return new WaitForSeconds(2f);

		if (healthCollected)
		{
			completed = true;
			ExitAction(succes);
		}
		else
		{
			agentMemory.HealthPacks.RemoveDetected(target);
			ExitAction(fail);
		}
	}

	

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Pickable"))
		{
			if (other.gameObject.CompareTag("HealthPack"))
			{
				if (!healthCollected)
				{
					agentMemory.AddHealth(50);
					agentMemory.HealthPacks.RemoveDetected(target);
					healthCollected = true;
				}
			}
			
		}
	}
	
	public override float GetCost()
	{
		if(agentMemory.healthAmount < 30)
		{
			cost = 1f;
		}
		else if(agentMemory.healthAmount < 50)
		{
			cost = 3f;
		}
		else if(agentMemory.healthAmount < 70)
		{
			cost = 6f;
		}
		else
		{
			cost = 10f;
		}

		return cost;
	}
}
