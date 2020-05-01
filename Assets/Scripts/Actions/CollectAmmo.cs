using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollectAmmo : GoapAction 
{

	private IGoap agent;
	private Memory agentMemory;
	private NavigationSystem agentNavigation;

	private bool ammoCollected = false;
	private bool completed = false;

	public CollectAmmo() 
	{
		name = "CollectAmmo";

		AddPrecondition(StateKeys.AMMO_AMOUNT, false);
		AddPrecondition(StateKeys.AMMO_DETECTED, true);

		AddEffect(StateKeys.AMMO_AMOUNT, true);

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
		ammoCollected = false;
		completed = false;
	}
	
	public override bool IsActionDone ()
	{
		return completed;
	}

	public override void SetActionTarget()
	{
		if (agentMemory.AmmoPacks.IsAnyValidDetected())
		{
			target = agentMemory.AmmoPacks.GetDetected();
			agentNavigation.SetTarget(target);
		}
	}
	

	public override bool CheckProceduralPrecondition (GameObject agentGo)
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
					cost = 10;
					agentMemory.AmmoPacks.InvalidateDetected(target);
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

		if (ammoCollected)
		{
			completed = true;
			ExitAction(succes);
		}
		else
		{
			agentMemory.AmmoPacks.RemoveDetected(target);
			ExitAction(fail);
		}
	}

	

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Pickable"))
		{
			if (!ammoCollected)
			{
				agentMemory.IncreaseAmmo();
				agentMemory.AmmoPacks.RemoveDetected(target);
				ammoCollected = true;
			}
		}
	}
	

}
