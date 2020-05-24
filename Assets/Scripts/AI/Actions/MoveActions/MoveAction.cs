using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class MoveAction : GoapAction
{
	protected IGoap agent;
	protected MemorySystem agentMemory;
	protected NavigationSystem agentNavigation;
	protected bool isActionExited = false;

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
	
	public override void EnterAction(Action Success, Action Fail, Action Reset)
	{
		isActionExited = false;
		actionCompleted = Success;
		actionFailed = Fail;
		actionReset = Reset;
		SetActionTarget();
		AddListeners();

	}
	public override void ExecuteAction(GameObject agent)
	{
		ExitAction(actionCompleted);
	}
	public void RestartAction()
	{
		ResetAction();
		agentNavigation.InvalidateTarget();
		actionReset.Invoke();
	}

	protected override void ExitAction(Action ExitAction)
	{
		if(!isActionExited)
		{
			isActionExited = true;
			RemoveListeners();
			IsActionDone = true;

			ExitAction?.Invoke();
			target = null;
			agentNavigation.InvalidateTarget();
			
		}
		
	}

	protected abstract void AddListeners();
	protected abstract void RemoveListeners();
}