using GOAP;
using System;
using System.Collections;
using UnityEngine;

public abstract class SearchAction : MoveAction 
{

	public override void SetActionTarget()
	{
		agent.Navigation.SetSearchTarget();
		target = agent.Navigation.Target;
	}

	public override void InvalidTargetLocation()
	{
		SetActionTarget();
	}

	public override void EnterAction(Action Success, Action Fail, Action Reset)
	{
		base.EnterAction(Success, Fail, Reset);
	}

	public override void ExecuteAction()
	{
		RestartAction();
	}

	protected override void ExitAction(Action ExitAction)
	{
		base.ExitAction(ExitAction);
	}
	
}
