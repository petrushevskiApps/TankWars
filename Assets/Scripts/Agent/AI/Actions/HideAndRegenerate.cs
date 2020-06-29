using GOAP;
using System;
using System.Collections;
using UnityEngine;

public class HideAndRegenerate : GoapAction 
{
	protected DetectedHolder detectedMemory;
	protected Coroutine UpdateCoroutine;

	private const float REGENERATE_INTERVAL = 1f;
	private const float HEALTH_PER_SECOND = 10;
	private const int   AMMO_PER_SECOND = 3;
	
	private void Start()
	{
		detectedMemory = agent.Memory.HidingSpots;
	}

	public HideAndRegenerate() 
	{
		AddPrecondition(StateKeys.ENEMY_DETECTED, false);
		AddPrecondition(StateKeys.HIDING_SPOT_DETECTED, true);

		AddEffect(StateKeys.HEALTH_FULL, true);
		AddEffect(StateKeys.AMMO_FULL, true);
	}

	/* PLANINING PHASE */
	public override bool CheckProceduralPreconditions()
	{
		GameObject hidingSpot = agent.Memory.HidingSpots.GetSortedDetected();

		Agent enemy = agent.Memory.Enemies.GetSortedDetected()?.GetComponent<Agent>();

		// Enemy in sight
		if (enemy != null)
		{
			// How many seconds until agent reaches pickable
			float timeToPickable = Utilities.TimeToReach(transform.position, hidingSpot, agent.Navigation.currentSpeed);

			// Total seconds until agent collects pickable
			float timeAgentToExecute = timeToPickable + 15f;

			// How many seconds until agent reaches agent
			float timeToEnemy = Utilities.TimeToReach(transform.position, enemy.gameObject, enemy.Navigation.currentSpeed);
			// How many seconds until enemy fires all ammo
			float timeToFullDamage = enemy.Inventory.Ammo.Amount * 0.5f;
			// Total time until enemy reaches agent and fires all ammo
			float timeToEnemyExecute = timeToEnemy + timeToFullDamage;

			// How many seconds can agent sustain damage
			float timeToDeath = (agent.Inventory.Health.Amount / 10) * 0.5f;

			if (timeAgentToExecute > timeToEnemyExecute)
			{
				return true;
			}
			else
			{
				// Agent can survive the attack
				if (timeToDeath - timeToFullDamage > 0)
				{
					return true;
				}
				// Agent can not survive attack
				else return false;
			}
		}
		else
		{
			// No enemies in sight
			return true;
		}
	}

	public override float GetCost()
	{

		float TTR = Utilities.TimeToReach(transform.position, agent.Memory.HidingSpots.GetSortedDetected(), agent.Navigation.currentSpeed);
		float IH = agent.Inventory.Health.GetInvertedCost();
		float IA = agent.Inventory.Ammo.GetCost();

		float time = Mathf.Clamp((15 + TTR) - (IH + IA), 0f, Mathf.Infinity);
		float cost = time * time;
		return Mathf.Clamp(cost, minimumCost, Mathf.Infinity);
	}

	/* LIFECYCLE PHASE */
	

	public override void SetActionTarget()
	{
		if (detectedMemory.IsAnyValidDetected())
		{
			target = detectedMemory.GetSortedDetected();
			agent.Navigation.SetTarget(target);
		}
		else
		{
			ActionAbort();
		}
	}
	public override void ResetTarget()
	{
		detectedMemory.InvalidateDetected(target);
		SetActionTarget();
	}

	
	public override void ExecuteAction()
	{
		if (!detectedMemory.IsDetectedValid(target))
		{
			ActionAbort();
		}
		else
		{
			UpdateCoroutine = StartCoroutine(ActionUpdate());
		}
	}
	
	protected IEnumerator ActionUpdate()
	{
		if (!detectedMemory.IsDetectedValid(target)) ActionAbort();

		while(agent.Inventory.Health.Status != InventoryStatus.Full)
		{
			agent.Inventory.Health.Increase(HEALTH_PER_SECOND);
			yield return new WaitForSeconds(REGENERATE_INTERVAL);
		}
		while(agent.Inventory.Ammo.Status != InventoryStatus.Full)
		{
			agent.Inventory.Ammo.Increase(AMMO_PER_SECOND);
			yield return new WaitForSeconds(REGENERATE_INTERVAL);
		}
		
		ExitAction(actionCompleted, 0f);
	}

	protected override void ExitAction(Action ExitAction, float invalidateTime)
	{
		if(!IsActionExited)
		{
			IsActionExited = true;
			IsActionDone = true;

			UnregisterListeners();

			if (UpdateCoroutine != null)
			{
				StopCoroutine(UpdateCoroutine);
				UpdateCoroutine = null;
			}
			
			agent.Navigation.InvalidateTarget();
			
			if(target != null && invalidateTime > 0)
			{
				detectedMemory.InvalidateDetected(target, invalidateTime);
				target = null;
			}

			ExitAction?.Invoke();
		}
	}

	protected override void RegisterListeners()
	{
		base.RegisterListeners();

		agent.Sensors.OnUnderAttack.AddListener(ActionAbort);
		agent.Sensors.OnEnemyDetected.AddListener(OnAgentDetected);
		agent.Sensors.OnFriendlyDetected.AddListener(OnAgentDetected);

		agent.Memory.HealthPacks.OnDetected.AddListener(ReplanningAbort);
		agent.Memory.AmmoPacks.OnDetected.AddListener(ReplanningAbort);
		agent.Memory.HidingSpots.OnDetected.AddListener(ReplanningAbort);
	}
	protected override void UnregisterListeners()
	{
		base.UnregisterListeners();

		agent.Sensors.OnUnderAttack.RemoveListener(ActionAbort);
		agent.Sensors.OnEnemyDetected.RemoveListener(OnAgentDetected);
		agent.Sensors.OnFriendlyDetected.RemoveListener(OnAgentDetected);

		agent.Memory.HealthPacks.OnDetected.RemoveListener(ReplanningAbort);
		agent.Memory.AmmoPacks.OnDetected.RemoveListener(ReplanningAbort);
		agent.Memory.HidingSpots.OnDetected.RemoveListener(ReplanningAbort);
	}

	// On agent detected, check if the agent is
	// executing same action. If it does, check
	// is it closer to target and abort and re-plan
	// otherwise continue
	private void OnAgentDetected(GameObject agent)
	{
		if (target != null && agent != null)
		{
			GoapAgent gaOther = agent.GetComponent<GoapAgent>();

			if (gaOther != null && gaOther.GetCurrentAction().Equals(ActionName))
			{
				CompareDistanceToTarget(agent);
			}
		}
	}

	private void CompareDistanceToTarget(GameObject otherAgent)
	{
		float otherToTargetDistance = Utilities.GetDistance(otherAgent, target);
		float agentToTargetDistance = Utilities.GetDistance(gameObject, target);

		if (agentToTargetDistance > otherToTargetDistance)
		{
			if (agentToTargetDistance < maxRequiredRange)
			{
				ActionAbort();
			}
		}
	}

}
