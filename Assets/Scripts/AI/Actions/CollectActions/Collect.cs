using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collect : GoapAction
{
    protected AiAgent agent;
    protected MemorySystem agentMemory;
    protected NavigationSystem agentNavigation;
    protected Detected detectedMemory;

    protected Coroutine UpdateCoroutine;

    protected void Start()
    {
        agent = GetComponent<AiAgent>();
        agentMemory = agent.GetMemory();
        agentNavigation = agent.GetNavigation();
    }

    public override void ResetAction()
    {
        base.ResetAction();
    }

    public override void SetActionTarget()
    {
        if (detectedMemory.IsAnyValidDetected())
        {
            target = detectedMemory.GetDetected();
            agentNavigation.SetTarget(target);
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

    public override bool TestProceduralPreconditions()
    {
        return true;
    }

    public bool CheckActionConditions()
    {
        return IsTargetValid();
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

        UpdateCoroutine = StartCoroutine(CollectPickable());
    }

    private IEnumerator CollectPickable()
    {
        if (detectedMemory.IsDetectedValid(target))
        {
            Pickable pickable = target.GetComponent<Pickable>();

            yield return new WaitForSeconds(pickable.GetTimeToCollect());

            pickable.Collect(agent.GetComponent<ICollector>());

            ExitAction(actionCompleted);
        }
        else
        {
            Invalidate();
            ExitAction(actionFailed);
        }

    }

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
        agentNavigation.InvalidateTarget();

        ExitAction?.Invoke();
    }

    protected virtual void AddListeners()
    {
        agent.GetPerceptor().OnEnemyDetected.AddListener(OnOtherDetected);
        agent.GetPerceptor().OnFriendlyDetected.AddListener(OnOtherDetected);
        agent.GetPerceptor().OnUnderAttack.AddListener(OnUnderAttack);
    }

    protected virtual void RemoveListeners()
    {
        agent.GetPerceptor().OnEnemyDetected.RemoveListener(OnOtherDetected);
        agent.GetPerceptor().OnFriendlyDetected.RemoveListener(OnOtherDetected);
        agent.GetPerceptor().OnUnderAttack.RemoveListener(OnUnderAttack);
    }

    private void OnUnderAttack(GameObject arg0)
    {
        Invalidate();
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
            Invalidate();
            ExitAction(actionFailed);
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

    protected void Invalidate()
    {
        float distance = GetDistanceToCollectible(gameObject);
        
        if (distance <= 15)
        {
            detectedMemory.InvalidateDetected(target);
        }
    }

}
