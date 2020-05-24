using Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliminateEnemy : GoapAction
{
	private AiAgent agent;
	private MemorySystem agentMemory;
	private NavigationSystem agentNavigation;

	private Coroutine LookAtCoroutine;
	private Coroutine FireAtCoroutine;


	public EliminateEnemy()
	{
		actionName = "EliminateEnemy";

		AddPrecondition(StateKeys.ENEMY_DETECTED, true);

		AddPrecondition(StateKeys.AMMO_AVAILABLE, true);
		AddPrecondition(StateKeys.HEALTH_AMOUNT, true);

		AddEffect(StateKeys.ENEMY_DETECTED, false);

	}
	private void Start()
	{
		agent = GetComponent<AiAgent>();
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
		else
		{
			ExitAction(actionCompleted);
		}
	}
	public override void InvalidTargetLocation()
	{
		SetActionTarget();
	}

	public override bool TestProceduralPreconditions()
	{
		return true;
	}

	public bool CheckActionConditions()
	{
		return agentMemory.IsAmmoAvailable()
			&& agentMemory.IsHealthAvailable();
	}

	public override void EnterAction(Action Success, Action Fail, Action Reset)
	{
		actionCompleted = Success;
		actionFailed = Fail;
		actionReset = Reset;

		SetActionTarget();
		AddListeners();
	}

	public override void ExecuteAction(GameObject agent)
	{
		LookAtCoroutine = StartCoroutine(agentNavigation.LookAtTarget());
		FireAtCoroutine = StartCoroutine(Fire());
	}

	protected override void ExitAction(Action ExitAction)
	{
		RemoveListeners();
		CancelCoroutines();
		ExitAction?.Invoke();
		IsActionDone = true;
		target = null;
		agentNavigation.InvalidateTarget();
		
	}

	
	IEnumerator Fire()
	{
		while (true)
		{ 
			if (CheckActionConditions())
			{
				if (target != null)
				{
					agent.GetWeapon().FireBullet(target);
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

	private void AddListeners()
	{
		agent.GetPerceptor().OnEnemyDetected.AddListener(EnemyDetected);
		agent.GetPerceptor().OnEnemyLost.AddListener(EnemyLost);
	}
	private void RemoveListeners()
	{
		agent.GetPerceptor().OnEnemyDetected.RemoveListener(EnemyDetected);
		agent.GetPerceptor().OnEnemyLost.RemoveListener(EnemyLost);
	}

	private void EnemyLost(GameObject enemy)
	{
		if(agentMemory.Enemies.IsAnyValidDetected())
		{
			SetActionTarget();
		}
		else
		{
			ExitAction(actionCompleted);
		}
	}

	private void EnemyDetected(GameObject enemy)
	{
		SetActionTarget();
	}


	private void CancelCoroutines()
	{
		if (LookAtCoroutine != null)
		{
			StopCoroutine(LookAtCoroutine);
			LookAtCoroutine = null;
		}

		if (FireAtCoroutine != null)
		{
			StopCoroutine(FireAtCoroutine);
			FireAtCoroutine = null;
		}
	}


}
