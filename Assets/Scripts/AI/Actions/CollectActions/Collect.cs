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

    protected Coroutine UpdateAction;

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
            UpdateAction = StartCoroutine(Collecting());
        }
        else
        {
            ExitAction(actionFailed);
        }
    }

    protected abstract IEnumerator Collecting();

    protected override void ExitAction(Action ExitAction)
    {
        if (UpdateAction != null)
        {
            StopCoroutine(UpdateAction);
            UpdateAction = null;
        }
        RemoveListeners();
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
        detectedMemory.InvalidateDetected(target);
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
        float otherDistanceToPacket = Vector3.Distance(otherPlayer.transform.position, target.transform.position);
        float distanceToPacket = Vector3.Distance(transform.position, target.transform.position);

        if (distanceToPacket > otherDistanceToPacket)
        {
            //agentMemory.AmmoPacks.RemoveDetected(target);
            detectedMemory.InvalidateDetected(target);
            ExitAction(actionFailed);
        }
    }
}
