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
		float agentHealth = agent.Inventory.Health.Amount;
		float agentAmmo = agent.Inventory.Ammo.Amount * 10;

		if (agentAmmo > 0)
		{
			// Default case no enemy detected during planning phase,
			// count enemy health and ammo as full.
			float enemyHealth = agent.Inventory.Health.Capacity;
			float enemyAmmo = agent.Inventory.Ammo.Capacity * 10;

			Agent enemy = agent.Memory.Enemies.GetSortedDetected()?.GetComponent<Agent>();

			if (enemy != null)
			{
				// In case enemy is detected during planning phase,
				// count real enemy health and ammo.
				enemyHealth = enemy.Inventory.Health.Amount;
				enemyAmmo = enemy.Inventory.Ammo.Amount * 10f;
			}

			float cost = (enemyHealth - agentAmmo) + (enemyAmmo - agentHealth);

			return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
		}
		else
		{
			return Mathf.Infinity;
		}
		
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
			ExitAction(actionCompleted, 0f);
		}
	}

	public override void ResetTarget()
	{
		ExitAction(actionReset, 4f);
	}

	private FireRangeStatus InFireRange()
	{
		float fireRange = Vector3.Distance(transform.position, target.transform.position);

		if (fireRange > maxRequiredRange) return FireRangeStatus.Follow;
		
		return fireRange < minRequiredRange ? FireRangeStatus.ToClose : FireRangeStatus.InRange;
	}

	public override void ExecuteAction()
	{
		agent.Navigation.LookAtTarget();
		agent.Navigation.FollowTarget(maxRequiredRange);
		FireCoroutine = StartCoroutine(Fire());
	}

	protected override void ExitAction(Action ExitAction, float invalidateTime)
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
			if (target != null)
			{
				agent.Navigation.InvalidateTarget();
				target.GetComponent<IDestroyable>()?.UnregisterOnDestroy(EnemyKilled);
				if(invalidateTime > 0)
				{
					agent.Memory.Enemies.InvalidateDetected(target, invalidateTime);
				}
				target = null;
			}

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
						ActionAbort();
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
		CustomAbort(2f);
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
		agent.Inventory.Health.OnStatusChange.AddListener(ReplanningAbort);
		//agent.Inventory.Ammo.OnStatusChange.AddListener(ReplanningAbort);
	}

	protected override void UnregisterListeners()
	{
		base.UnregisterListeners();

		agent.Inventory.Health.OnStatusChange.RemoveListener(ReplanningAbort);
		//agent.Inventory.Ammo.OnStatusChange.RemoveListener(ReplanningAbort);
	}

	private void EnemyKilled(GameObject arg0)
	{
		agent.Memory.Enemies.RemoveDetected(target);
		ExitAction(actionCompleted, 0f);
	}

}
