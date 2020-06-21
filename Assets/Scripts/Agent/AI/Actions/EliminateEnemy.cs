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
	private bool isActionExited = false;

	public EliminateEnemy()
	{
		//actionName = "KillEnemy";

		AddPrecondition(StateKeys.ENEMY_DETECTED, true);

		AddEffect(StateKeys.ENEMY_KILLED, true);
	}

	public override float GetCost()
	{
		float E  = GetEnemyCost(agent.Memory.Enemies);
		float IH = agent.Inventory.Health.GetCost();
		float IA = agent.Inventory.Ammo.GetCost();

		float cost = E + (IH * IH) + IA;

		return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
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

	public override void InvalidTargetLocation()
	{
		ExitAction(actionReset);
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
		this.agent.Navigation.LookAtTarget();
		this.agent.Navigation.FollowTarget(maxRequiredRange);
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
			target = null;
			agent.Navigation.InvalidateTarget();
		}
	}

	
	IEnumerator Fire()
	{
		while (true)
		{
			if (target != null)
			{
				if (CheckActionRange() == FireRangeStatus.ToClose)
				{
					agent.Memory.Enemies.InvalidateDetected(target, 2f);
					ExitAction(actionFailed);
					break;
				}

				if (CheckActionRange() == FireRangeStatus.InRange)
				{
					if (agent.Navigation.IsLookingAtTarget())
					{
						if(agent.Inventory.Ammo.Amount > 0)
						{
							agent.Weapon.FireBullet();
							yield return new WaitForSeconds(0.5f);
						}
						else
						{
							ExitAction(actionFailed);
							break;
						}
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
	}

	private void AddListeners()
	{
		agent.Memory.Enemies.OnDetected.AddListener(EnemyDetected);
		agent.Inventory.Health.OnStatusChange.AddListener(HealthStatusChange);
		agent.Inventory.Ammo.OnStatusChange.AddListener(AmmoStatusChange);
	}
	private void RemoveListeners()
	{
		agent.Memory.Enemies.OnDetected.RemoveListener(EnemyDetected);
		agent.Inventory.Health.OnStatusChange.RemoveListener(HealthStatusChange);
		agent.Inventory.Ammo.OnStatusChange.RemoveListener(AmmoStatusChange);
	}

	private void HealthStatusChange(InventoryStatus status)
	{
		// If Health Inventory is low before enemy
		// is destoryed action failed ( re - plan )
		if (status == InventoryStatus.Low)
		{
			ExitAction(actionFailed);
		}
	}

	private void AmmoStatusChange(InventoryStatus invStatus)
	{
		// If Ammo Inventory is empty before enemy
		// is destoryed action failed ( re - plan )
		if(invStatus == InventoryStatus.Empty )
		{
			ExitAction(actionFailed);
		}
	}


	private void EnemyDetected()
	{
		ExitAction(actionFailed);
	}
	

}
