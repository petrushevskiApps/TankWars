using GOAP;
using System;
using System.Collections;
using UnityEngine;

public abstract class Collect : GoapAction
{
    protected AIAgent agent;
    protected DetectedHolder detectedMemory;

    protected Coroutine UpdateCoroutine;

    protected void Start()
    {
        agent = GetComponent<AIAgent>();
    }

    public override bool CheckProceduralPreconditions()
    {
        GameObject pickable = detectedMemory.GetSortedDetected();

        Agent enemy = agent.Memory.Enemies.GetSortedDetected()?.GetComponent<Agent>();

        // Enemy in sight
        if (enemy != null && pickable != null)
        {
            // How many seconds until agent reaches pickable
            float timeToPickable = Utilities.TimeToReach(transform.position, pickable, agent.Navigation.currentSpeed);

            // Total seconds until agent collects pickable
            float timeAgentToExecute = timeToPickable + pickable.GetComponent<Pickable>().timeToCollect;

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
                else
                {
                    detectedMemory.InvalidateDetected(pickable, 4f);
                    return false;
                }
            }
        }
        else
        {
            // No enemies in sight
            return true;
        }
    }


    public override void SetActionTarget()
    {
        if (detectedMemory.IsAnyValidDetected())
        {
            target = detectedMemory.GetSortedDetected();
            agent.Navigation.SetTarget(target);
        }
        else
        {
            ExitAction(actionFailed);
        }
    }

    public override void InvalidTargetLocation()
    {
        detectedMemory.InvalidateDetected(target);
        SetActionTarget();
    }

    public override void EnterAction(Action Success, Action Fail, Action Reset)
    {
        IsActionExited = false;
        IsActionDone = false;

        actionCompleted = Success;
        actionFailed = Fail;
        actionReset = Reset;

        SetActionTarget();

        AddListeners();
    }

    public override void ExecuteAction()
    {
        CollectController collector = agent.GetComponent<Agent>().Collector;

        if (collector != null && collector.IsPickableReady)
        {
            collector.CollectPickable(true);
            UpdateCoroutine = StartCoroutine(CollectPickable());
        }
        else
        {
            ExitAction(actionFailed);
        }
    }


    protected abstract IEnumerator CollectPickable();


    protected override void ExitAction(Action ExitAction)
    {
        if(!IsActionExited)
        {
            IsActionExited = true;
            IsActionDone = true;

            RemoveListeners();

            if (UpdateCoroutine != null)
            {
                StopCoroutine(UpdateCoroutine);
                UpdateCoroutine = null;
            }

            if (target != null)
            {
                detectedMemory.InvalidateDetected(target);
                target = null;
            }
            agent.Navigation.InvalidateTarget();

            ExitAction?.Invoke();
        }
        
    }

    protected virtual void AddListeners()
    {
        agent.Memory.Enemies.OnDetected.AddListener(AbortAction);
        agent.Sensors.OnEnemyDetected.AddListener(OnAgentDetected);
        agent.Sensors.OnFriendlyDetected.AddListener(OnAgentDetected);
        agent.Sensors.OnUnderAttack.AddListener(OnUnderAttack);
        
        detectedMemory.OnDetected.AddListener(OnNewDetected);
        agent.Memory.HidingSpots.OnDetected.AddListener(AbortAction);
    }

    protected virtual void RemoveListeners()
    {
        agent.Memory.Enemies.OnDetected.RemoveListener(AbortAction);
        agent.Sensors.OnEnemyDetected.RemoveListener(OnAgentDetected);
        agent.Sensors.OnFriendlyDetected.RemoveListener(OnAgentDetected);
        agent.Sensors.OnUnderAttack.RemoveListener(OnUnderAttack);

        detectedMemory.OnDetected.RemoveListener(OnNewDetected);
        agent.Memory.HidingSpots.OnDetected.RemoveListener(AbortAction);
    }

    // When attacked  re-plan
    private void OnUnderAttack(GameObject arg0)
    {
        AbortAction();
    }
    private void AbortAction()
    {
        ExitAction(actionFailed);
    }

    // When new pickable is detected
    // resetting target will re-sort detected
    // pickables and update target to the closest one
    protected void OnNewDetected()
    {
        SetActionTarget();
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
                ExitAction(actionFailed);
            }
        }
    }
}
