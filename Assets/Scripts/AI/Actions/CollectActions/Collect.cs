using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collect : GoapAction
{
    protected IGoap agent;
    protected MemorySystem agentMemory;
    protected NavigationSystem agentNavigation;
    protected Detected detectedMemory;

    protected Coroutine UpdateCoroutine;

    protected void Start()
    {
        agent = GetComponent<IGoap>();
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
    }

    protected bool IsTargetValid()
    {
        return target != null && target.activeSelf;
    }

    public override bool CheckPreconditions(GameObject agentGo)
    {
        return IsTargetValid();
    }

    public override void EnterAction(Action success, Action fail)
    {
        actionCompleted = success;
        actionFailed = fail;

        SetActionTarget();

        AddListeners();
    }

    public override void ExecuteAction(GameObject agent)
    {
        Debug.Log($"<color=green> {gameObject.name} Perform Action: {actionName}</color>");

        if (IsTargetValid())
        {
            UpdateCoroutine = StartCoroutine(ActionUpdate());
        }
        else
        {
            ExitAction(actionFailed);
        }
    }

    protected abstract IEnumerator ActionUpdate();

    protected override void ExitAction(Action ExitAction)
    {
        RemoveListeners();

        if (UpdateCoroutine != null)
        {
            StopCoroutine(UpdateCoroutine);
            UpdateCoroutine = null;
        }
        
        IsActionDone = true;
        target = null;
        agentNavigation.InvalidateTarget();
        ExitAction?.Invoke();
    }

    private void AddListeners()
    {
        agent.GetPerceptor().OnEnemyDetected.AddListener(OnOtherDetected);
        agent.GetPerceptor().OnFriendlyDetected.AddListener(OnOtherDetected);
        agent.GetPerceptor().OnUnderAttack.AddListener(OnUnderAttack);
        agent.GetPerceptor().OnFriendlyFire.AddListener(OnUnderAttack);
    }

    private void RemoveListeners()
    {
        agent.GetPerceptor().OnEnemyDetected.RemoveListener(OnOtherDetected);
        agent.GetPerceptor().OnFriendlyDetected.RemoveListener(OnOtherDetected);
        agent.GetPerceptor().OnUnderAttack.RemoveListener(OnUnderAttack);
        agent.GetPerceptor().OnFriendlyFire.AddListener(OnUnderAttack);
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

    private void Invalidate()
    {
        float distance = GetDistanceToCollectible(gameObject);
        
        if (distance <= 15)
        {
            detectedMemory.InvalidateDetected(target);
        }
    }
}
