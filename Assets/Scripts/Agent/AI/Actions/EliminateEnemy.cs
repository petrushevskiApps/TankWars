using Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EliminateEnemy : GoapAction
{
	private AIAgent agent;

	private bool isActionExited = false;
	private Coroutine FireCoroutine;
	private Coroutine TimedAbort;

	private void Awake()
	{
		agent = GetComponent<AIAgent>();
	}

	public EliminateEnemy()
	{
		AddPrecondition(StateKeys.ENEMY_DETECTED, true);

		AddEffect(StateKeys.ENEMY_KILLED, true);
		AddEffect(StateKeys.ENEMY_DETECTED, false);
	}

	public override float GetCost()
	{
		Agent enemy = agent.Memory.Enemies.GetSortedDetected()?.GetComponent<Agent>();

		// Default case no enemy detected during planning phase,
		// count enemy health and ammo as full.
		float enemyHealthTime = (agent.Inventory.Health.Capacity / 10) * 0.5f;
		float enemyAmmoTime = agent.Inventory.Ammo.Capacity * 0.5f;

		if (enemy != null)
		{
			// In case enemy is detected during planning phase,
			// count real enemy health and ammo.
			enemyHealthTime = (enemy.Inventory.Health.Amount / 10) * 0.5f;
			enemyAmmoTime = enemy.Inventory.Ammo.Amount * 0.5f;
		}

		float agentHealthTime = (agent.Inventory.Health.Amount / 10) * 0.5f;
		float agentAmmoTime = agent.Inventory.Ammo.Amount * 0.5f;
		
		float cost = (enemyHealthTime - agentAmmoTime) + (enemyAmmoTime - agentHealthTime) + ( 5 - agentAmmoTime );

		return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
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

	public override void InvalidTargetLocation()
	{
		ExitAction(actionReset);
	}

	private FireRangeStatus InFireRange()
	{
		float fireRange = Vector3.Distance(transform.position, target.transform.position);

		return fireRange < minRequiredRange ? FireRangeStatus.ToClose : FireRangeStatus.InRange;

	}

	public override void EnterAction(Action Success, Action Fail, Action Reset)
	{
		isActionExited = false;
		IsActionDone = false;

		actionCompleted = Success;
		actionFailed = Fail;
		actionReset = Reset;

		SetActionTarget();
		AddListeners();
	}

	public override void ExecuteAction()
	{
		agent.Navigation.LookAtTarget();
		agent.Navigation.FollowTarget(maxRequiredRange);
		FireCoroutine = StartCoroutine(Fire());
	}

	protected override void ExitAction(Action ExitAction)
	{
		if(!isActionExited)
		{
			isActionExited = true;

			RemoveListeners();
			agent.Navigation.StopLookAt();
			agent.Navigation.StopFollow();

			if (FireCoroutine != null)
			{
				StopCoroutine(FireCoroutine);
				FireCoroutine = null;
			}

			IsActionDone = true;

			ExitAction?.Invoke();
			
			if(target != null)
			{
				target = null;
			}
			agent.Navigation.InvalidateTarget();
		}
	}

	

	IEnumerator Fire()
	{
		while (target != null)
		{
			if (InFireRange() == FireRangeStatus.ToClose)
			{
				if (TimedAbort == null)
				{
					TimedAbort = StartCoroutine(StartTimedAbort(1f));
				}
			}
			else if (InFireRange() == FireRangeStatus.InRange)
			{
				StopTimedAbort();

				if (agent.Navigation.IsLookingAtTarget())
				{
					if (agent.Inventory.Ammo.Amount > 0)
					{
						agent.Weapon.FireBullet();
					}
					else
					{
						agent.Memory.Enemies.InvalidateDetected(target, 4f);
						ExitAction(actionFailed);
					}
				}
			}

			yield return new WaitForSeconds(0.01f);
		}
		
		agent.Memory.Enemies.RemoveDetected(target);
		ExitAction(actionCompleted);
	}
	IEnumerator StartTimedAbort(float time)
	{
		yield return new WaitForSeconds(time);
		agent.Memory.Enemies.InvalidateDetected(target, 4f);
		ExitAction(actionFailed);
	}
	private void StopTimedAbort()
	{
		if(TimedAbort != null)
		{
			StopCoroutine(TimedAbort);
			TimedAbort = null;
		}
	}

	private void AddListeners()
	{
		agent.Memory.Enemies.OnDetected.AddListener(AbortAction);
		agent.Inventory.Health.OnStatusChange.AddListener(AbortAction);
		agent.Inventory.Ammo.OnStatusChange.AddListener(AbortAction);
	}
	
	private void RemoveListeners()
	{
		agent.Memory.Enemies.OnDetected.RemoveListener(AbortAction);
		agent.Inventory.Health.OnStatusChange.RemoveListener(AbortAction);
		agent.Inventory.Ammo.OnStatusChange.RemoveListener(AbortAction);
	}
	
	private void AbortAction()
	{
		ExitAction(actionFailed);
	}
}
