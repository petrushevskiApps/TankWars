using Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliminateEnemy : GoapAction
{
	private Agent agent;
	private MemorySystem agentMemory;
	private NavigationSystem agentNavigation;

	
	public EliminateEnemy()
	{
		actionName = "EliminateEnemy";

		AddPrecondition(StateKeys.ENEMY_DETECTED, true);
		AddPrecondition(StateKeys.IN_SHOOTING_RANGE, true);
		AddPrecondition(StateKeys.AMMO_AMOUNT, true);
		AddPrecondition(StateKeys.HEALTH_AMOUNT, true);

		AddEffect(StateKeys.ENEMY_DETECTED, false);

	}
	private void Start()
	{
		agent = GetComponent<Agent>();
		agentMemory = agent.GetMemory();
		agentNavigation = agent.GetNavigation();
	}

	public override void ResetAction()
	{
		base.ResetAction();
	}

	public override void SetActionTarget()
	{
		if (agentMemory.Enemies.IsAnyValidDetected())
		{
			target = agentMemory.Enemies.GetDetected();
			agentNavigation.SetTarget(target);
		} 
	}

	public override bool CheckPreconditions(GameObject agentGO)
	{
		return agentMemory.Enemies.IsAnyValidDetected() && agent.GetInventory().IsAmmoAvailable();
	}

	public override void EnterAction(Action success, Action fail)
	{
		actionCompleted = success;
		actionFailed = fail;
		SetActionTarget();
	}

	public override void ExecuteAction(GameObject agent)
	{
		StartCoroutine(agentNavigation.LookAtTarget(target));
		StartCoroutine(Fire(agent));
	}

	protected override void ExitAction(Action exitAction)
	{
		IsActionDone = true;
		target = null;
		agentNavigation.InvalidateTarget();
		exitAction?.Invoke();
	}

	IEnumerator Fire(GameObject agent)
	{
		while (true)
		{ 
			if (CheckPreconditions(agent))
			{
				if (target != null)
				{
					this.agent.GetWeapon().FireBullet(target);
					yield return new WaitForSeconds(0.5f);
				}
				else
				{
					agentMemory.Enemies.RemoveDetected(target);
					ExitAction(actionCompleted);
					break;
				}
			}
			else
			{
				ExitAction(actionFailed);
				break;
			}
		}
	}

	

	
}
