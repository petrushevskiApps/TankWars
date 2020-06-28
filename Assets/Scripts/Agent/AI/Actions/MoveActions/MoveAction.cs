using System;
using UnityEngine;

public abstract class MoveAction : GoapAction
{
	protected AIAgent agent;

	private void Start()
	{
		agent = GetComponent<AIAgent>();
	}
	public override bool CheckProceduralPreconditions()
	{
		return true;
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
		ExitAction(actionCompleted);
	}

	// Restart Moving action without re-planning
	// if it didn't found or detected anything
	// while moving
	public void RestartAction()
	{
		ResetAction();
		agent.Navigation.InvalidateTarget();
		actionReset.Invoke();
	}

	protected override void ExitAction(Action ExitAction)
	{
		if(!IsActionExited)
		{
			RemoveListeners();
			IsActionExited = true;
			IsActionDone = true;

			ExitAction?.Invoke();
			target = null;
			agent.Navigation.InvalidateTarget();
			
		}
		
	}

	protected abstract void AddListeners();
	protected abstract void RemoveListeners();
}