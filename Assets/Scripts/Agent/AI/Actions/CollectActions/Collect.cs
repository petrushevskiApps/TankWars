using System;
using System.Collections;
using System.Collections.Generic;
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
        actionCompleted = Success;
        actionFailed = Fail;
        actionReset = Reset;

        SetActionTarget();

        AddListeners();
    }

    public override void ExecuteAction()
    {
        CollectController collector = agent.GetComponent<Agent>().Collector;

        if (collector!= null && collector.IsPickableReady)
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
        RemoveListeners();
        IsActionDone = true;

        if (UpdateCoroutine != null)
        {
            StopCoroutine(UpdateCoroutine);
            UpdateCoroutine = null;
        }

        target = null;
        agent.Navigation.InvalidateTarget();

        ExitAction?.Invoke();
    }

    protected virtual void AddListeners()
    {
        agent.Memory.HidingSpots.OnDetected.AddListener(HidingSpotDetected);
        
        detectedMemory.OnDetected.AddListener(OnNewDetected);

        agent.Sensors.OnEnemyDetected.AddListener(OnAgentDetected);
        agent.Sensors.OnFriendlyDetected.AddListener(OnAgentDetected);

        agent.Sensors.OnUnderAttack.AddListener(OnUnderAttack);
    }

    protected virtual void RemoveListeners()
    {
        agent.Memory.HidingSpots.OnDetected.RemoveListener(HidingSpotDetected);
        detectedMemory.OnDetected.RemoveListener(OnNewDetected);

        agent.Sensors.OnEnemyDetected.RemoveListener(OnAgentDetected);
        agent.Sensors.OnFriendlyDetected.RemoveListener(OnAgentDetected);

        agent.Sensors.OnUnderAttack.RemoveListener(OnUnderAttack);
    }

    // When attacked abort collecting and
    // re-plan accordingly
    private void OnUnderAttack(GameObject arg0)
    {
        if (target != null)
        {
            detectedMemory.InvalidateDetected(target);
        }
        ExitAction(actionFailed);
    }

    // When new pickable is detected
    // resetting target will re-sort detected
    // pickables and update target to the closest one
    protected void OnNewDetected()
    {
        SetActionTarget();
    }

    // When hidding spot is detected and
    // both health and ammo are low, abort
    // action and re-plan.
    private void HidingSpotDetected()
    {
        if (!agent.Memory.IsHealthFull() && !agent.Memory.IsAmmoFull())
        {
            detectedMemory.InvalidateDetected(target);
            ExitAction(actionFailed);
        }
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
                CompareDistanceToPacket(agent);
            }
        }
    }

    private void CompareDistanceToPacket(GameObject otherAgent)
    {
        float otherDistanceToPacket = GetDistanceToCollectible(otherAgent);
        float distanceToPacket = GetDistanceToCollectible(gameObject);

        if (distanceToPacket > otherDistanceToPacket)
        {
            if (distanceToPacket < 20)
            {
                detectedMemory.InvalidateDetected(target);
                ExitAction(actionFailed);
            }
        }
    }

    public float GetDistanceToCollectible(GameObject agent)
    {
        if (agent != null && target != null)
        {
            return Vector3.Distance(agent.transform.position, target.transform.position);
        }
        else
        {
            return Mathf.Infinity;
        }
    }
}
