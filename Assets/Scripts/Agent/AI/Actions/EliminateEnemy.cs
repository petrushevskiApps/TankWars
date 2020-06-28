using System;
using System.Collections;
using UnityEngine;

public class EliminateEnemy : GoapAction
{
	private Coroutine FireCoroutine;
	private Coroutine TimedAbort;

	public EliminateEnemy()
	{
		AddPrecondition(StateKeys.ENEMY_DETECTED, true);

		AddEffect(StateKeys.ENEMY_KILLED, true);
		AddEffect(StateKeys.ENEMY_DETECTED, false);
	}

	/* PLANINING PHASE */
	public override bool CheckProceduralPreconditions()
	{
		return true;
	}

	public override float GetCost()
	{
		Agent enemy = agent.Memory.Enemies.GetSortedDetected()?.GetComponent<Agent>();

		// Default case no enemy detected during planning phase,
		// count enemy health and ammo as full.
		float enemyHealth = agent.Inventory.Health.Capacity;
		float enemyAmmo = agent.Inventory.Ammo.Capacity * 10;

		if (enemy != null)
		{
			// In case enemy is detected during planning phase,
			// count real enemy health and ammo.
			enemyHealth = enemy.Inventory.Health.Amount;
			enemyAmmo = enemy.Inventory.Ammo.Amount * 10f;
		}

		float agentHealth = agent.Inventory.Health.Amount;
		float agentAmmo = agent.Inventory.Ammo.Amount * 10;
		
		float cost = (enemyHealth - agentAmmo) + (enemyAmmo - agentHealth);

		return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
	}

	/* LIFECYCLE PHASE */
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

	public override void ResetTarget()
	{
		ExitAction(actionReset);
	}

	private FireRangeStatus InFireRange()
	{
		float fireRange = Vector3.Distance(transform.position, target.transform.position);

		return fireRange < minRequiredRange ? FireRangeStatus.ToClose : FireRangeStatus.InRange;
	}

	public override void ExecuteAction()
	{
		agent.Navigation.LookAtTarget();
		agent.Navigation.FollowTarget(maxRequiredRange);
		FireCoroutine = StartCoroutine(Fire());
	}

	protected override void ExitAction(Action ExitAction)
	{
		if(!IsActionExited)
		{
			UnregisterListeners();

			IsActionExited = true;
			IsActionDone = true;
			
			agent.Navigation.StopLookAt();
			agent.Navigation.StopFollow();

			if (FireCoroutine != null)
			{
				StopCoroutine(FireCoroutine);
				FireCoroutine = null;
			}

			agent.Navigation.InvalidateTarget();
			ExitAction?.Invoke();
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
					TimedAbort = StartCoroutine(StartTimedAbort(2f));
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
						InvalidateAndAbort();
					}
				}
			}
			else
			{
				StopTimedAbort();
			}

			yield return new WaitForSeconds(0.01f);
		}
		
		
	}
	IEnumerator StartTimedAbort(float time)
	{
		yield return new WaitForSeconds(time);
		InvalidateAndAbort();
	}
	private void StopTimedAbort()
	{
		if(TimedAbort != null)
		{
			StopCoroutine(TimedAbort);
			TimedAbort = null;
		}
	}

	protected override void RegisterListeners()
	{
		base.RegisterListeners();

		if (target != null)
		{
			target.GetComponent<IDestroyable>()?.RegisterOnDestroy(EnemyKilled);
		}
		//agent.Sensors.OnUnderAttack.AddListener(AbortAction);
		agent.Inventory.Health.OnStatusChange.AddListener(AbortAction);
		agent.Inventory.Ammo.OnStatusChange.AddListener(AbortAction);
	}

	protected override void UnregisterListeners()
	{
		base.UnregisterListeners();

		//agent.Sensors.OnUnderAttack.RemoveListener(AbortAction);
		agent.Inventory.Health.OnStatusChange.RemoveListener(AbortAction);
		agent.Inventory.Ammo.OnStatusChange.RemoveListener(AbortAction);
	}

	private void EnemyKilled(GameObject arg0)
	{
		agent.Memory.Enemies.RemoveDetected(target);
		ExitAction(actionCompleted);
	}

	private void InvalidateAndAbort()
	{
		if (target != null)
		{
			target.GetComponent<IDestroyable>()?.UnregisterOnDestroy(EnemyKilled);
			agent.Memory.Enemies.InvalidateDetected(target, 4f);
			target = null;
		}
		AbortAction();
	}
}
