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
	private Action actionCompleted;

	public CollectHealth() 
	{
		actionName = "CollectHealth";

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
		actionCompleted = null;
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
		Debug.Log($"<color=green> {gameObject.name} Perform Action: {actionName}</color>");

		if (!healthCollected)
		{
			ExitAction(fail);
		}
		else
		{
			actionCompleted = success;
		}
	}


	protected override void ExitAction(Action exitAction)
	{
		completed = true;
		agentMemory.AmmoPacks.RemoveDetected(target);
		agentNavigation.InvalidateTarget();
		exitAction?.Invoke();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Pickable"))
		{
			if (other.gameObject.CompareTag("HealthPack"))
			{
				healthCollected = true;
				other.gameObject.GetComponent<Pickable>().OnCollected.AddListener(OnCollectedHandler);
			}
			
		}
	}
	private void OnCollectedHandler(GameObject go)
	{
		ExitAction(actionCompleted);
	}

	public override float GetCost()
	{
		if(agent.GetInventory().GetHealth() < 30)
		{
			cost = 1f;
		}
		else if(agent.GetInventory().GetHealth() < 50)
		{
			cost = 3f;
		}
		else if(agent.GetInventory().GetHealth() < 70)
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
