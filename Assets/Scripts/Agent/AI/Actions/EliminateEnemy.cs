using Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EliminateEnemy : GoapAction
{
	private AIAgent agent;

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
	}

	public override void SetActionTarget()
	{
		if (agent.Memory.Enemies.IsAnyValidDetected())
		{
			target = agent.Memory.Enemies.GetSortedDetected();
			agent.Navigation.SetTarget(target);
		}
		else
		{
			ExitAction(actionCompleted);
		}
	}

	

	private FireRangeStatus CheckActionRange()
	{
		float fireRange = Vector3.Distance(transform.position, target.transform.position);

		if (fireRange > maxRequiredRange) return FireRangeStatus.Follow;
		else if (fireRange < minRequiredRange) return FireRangeStatus.ToClose;
		else return FireRangeStatus.InRange;
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
		this.agent.Navigation.LookAtTarget();
		this.agent.Navigation.FollowTarget(maxRequiredRange);
		FireCoroutine = StartCoroutine(Fire());
	}

	protected override void ExitAction(Action ExitAction)
	{
		RemoveListeners();

		if (FireCoroutine != null)
		{
			StopCoroutine(FireCoroutine);
			FireCoroutine = null;
		}

		IsActionDone = true;

		ExitAction?.Invoke();
		target = null;
		agent.Navigation.InvalidateTarget();
	}

	
	IEnumerator Fire()
	{
		while (true)
		{ 
			if (agent.Memory.IsAmmoAvailable() && agent.Memory.IsHealthAvailable())
			{
				if (target != null)
				{
					if(CheckActionRange() == FireRangeStatus.ToClose)
					{
						agent.Memory.Enemies.InvalidateDetected(target, 2f);
						ExitAction(actionFailed);
						break;
					}

					if (CheckActionRange() == FireRangeStatus.InRange)
					{
						if (agent.Navigation.IsLookingAtTarget())
						{
							agent.Weapon.FireBullet();
							yield return new WaitForSeconds(0.5f);
						}
					}
					yield return new WaitForSeconds(0.01f);
				}
				else
				{
					agent.Memory.Enemies.RemoveDetected(target);
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
		agent.Memory.Enemies.OnDetected.AddListener(EnemyDetected);
	}
	private void RemoveListeners()
	{
		agent.Memory.Enemies.OnDetected.RemoveListener(EnemyDetected);
	}

	private void EnemyDetected()
	{
		ExitAction(actionReset);
	}
	public override void InvalidTargetLocation()
	{
		ExitAction(actionReset);
	}

}
