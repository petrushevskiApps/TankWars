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

    protected bool IsTargetValid()
    {
        return target != null && target.activeSelf;
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
        Debug.Log($"<color=green> {gameObject.name} Perform Action: {actionName}</color>");

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
        agent.Sensors.OnEnemyDetected.AddListener(OnOtherDetected);
        agent.Sensors.OnFriendlyDetected.AddListener(OnOtherDetected);
        agent.Sensors.OnUnderAttack.AddListener(OnUnderAttack);
    }

    protected virtual void RemoveListeners()
    {
        agent.Sensors.OnEnemyDetected.RemoveListener(OnOtherDetected);
        agent.Sensors.OnFriendlyDetected.RemoveListener(OnOtherDetected);
        agent.Sensors.OnUnderAttack.RemoveListener(OnUnderAttack);
    }

    private void OnUnderAttack(GameObject arg0)
    {
        if (target != null)
        {
            detectedMemory.InvalidateDetected(target);
        }
        ExitAction(actionFailed);
    }

    private void OnOtherDetected(GameObject other)
    {
        if (target != null && other != null)
        {
            GoapAgent gaOther = other.GetComponent<GoapAgent>();

            if (gaOther != null && gaOther.GetCurrentAction().Equals(actionName))
            {
                CompareDistanceToPacket(other);
            }
        }
    }


    private void CompareDistanceToPacket(GameObject otherPlayer)
    {
        float otherDistanceToPacket = GetDistanceToCollectible(otherPlayer);
        float distanceToPacket = GetDistanceToCollectible(gameObject);

        if (distanceToPacket > otherDistanceToPacket)
        {
            if (distanceToPacket < 21)
            {
                detectedMemory.InvalidateDetected(target);
                ExitAction(actionFailed);
            }
        }
    }

    public float GetDistanceToCollectible(GameObject player)
    {
        if(player != null && target != null)
        {
            return Vector3.Distance(player.transform.position, target.transform.position);
        }
        else
        {
            return Mathf.Infinity;
        }
    }


}
