using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class MoveAction : GoapAction
{
	protected IGoap agent;
	protected Memory agentMemory;
	protected NavigationSystem agentNavigation;

	private void Start()
	{
		agent = GetComponent<IGoap>();
		agentMemory = agent.GetMemory();
		agentNavigation = agent.GetNavigation();
	}

	public override void ResetAction()
	{
		base.ResetAction();
	}

	public override void EnterAction(Action success, Action fail)
	{
		actionCompleted = success;
		actionFailed = fail;
		SetActionTarget();

	}
	public override void ExecuteAction(GameObject agent)
	{
		ExitAction(actionCompleted);
	}

	protected override void ExitAction(Action ExitAction)
	{
		IsActionDone = true;
		target = null;
		agentNavigation.InvalidateTarget();
		ExitAction?.Invoke();
	}
}