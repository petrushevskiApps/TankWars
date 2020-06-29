using System;
using UnityEngine;

public abstract class MoveAction : GoapAction
{
	public override bool CheckProceduralPreconditions()
	{
		return true;
	}
	public override void ResetTarget()
	{
		SetActionTarget();
	}

	public override void ExecuteAction()
	{
		ExitAction(actionCompleted, 0f);
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

	protected override void ExitAction(Action ExitAction, float invalidateTime)
	{
		if(!IsActionExited)
		{
			UnregisterListeners();
			IsActionExited = true;
			IsActionDone = true;

			ExitAction?.Invoke();
			target = null;
			agent.Navigation.InvalidateTarget();
			
		}
		
	}

}