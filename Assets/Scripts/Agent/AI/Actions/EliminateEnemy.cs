using Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EliminateEnemy : GoapAction
{

	public enum FireRangeStatus
	{
		InPosition,
		Follow,
		ToClose
	}
	private AIAgent agent;
	private MemorySystem agentMemory;
	private NavigationSystem agentNavigation;

	public UnityEvent OnEnemyKilled = new UnityEvent();
	public UnityEvent OnEnemyAttacked = new UnityEvent();

	private Coroutine FireCoroutine;

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
		agent = GetComponent<AIAgent>();
		agentMemory = agent.Memory;
		agentNavigation = agent.Navigation;
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

	private bool CheckActionConditions()
	{
		return agentMemory.IsAmmoAvailable()
			&& agentMemory.IsHealthAvailable();
	}
	private FireRangeStatus CheckActionRange()
	{
		float fireRange = Vector3.Distance(transform.position, target.transform.position);

		if (fireRange > maxRequiredRange) return FireRangeStatus.Follow;
		else if (fireRange < minRequiredRange) return FireRangeStatus.ToClose;
		else return FireRangeStatus.InPosition;
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
		StartCoroutine(agentNavigation.LookAtTarget());
		StartCoroutine(agentNavigation.Follow(maxRequiredRange));

		FireCoroutine = StartCoroutine(Fire());
	}

	protected override void ExitAction(Action ExitAction)
	{
		RemoveListeners();
		CancelCoroutines();
		IsActionDone = true;

		ExitAction?.Invoke();
		target = null;
		agentNavigation.InvalidateTarget();
		
	}

	
	IEnumerator Fire()
	{
		OnEnemyAttacked.Invoke();
		while (true)
		{ 
			if (CheckActionConditions())
			{
				
				if (target != null)
				{
					if(CheckActionRange() == FireRangeStatus.ToClose)
					{
						agentMemory.Enemies.InvalidateDetected(target);
						ExitAction(actionFailed);
						break;
					}
					else
					{
						if(CheckActionRange() == FireRangeStatus.InPosition)
						{
							if(agentNavigation.CheckAngle(target))
							{
								agent.GetWeapon().FireBullet();
							}
						}
					}

					yield return new WaitForSeconds(0.5f);
				}
				else
				{
					OnEnemyKilled.Invoke();
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
		if(FireCoroutine != null)
		{
			StopCoroutine(FireCoroutine);
			FireCoroutine = null;
		}
	}


}
