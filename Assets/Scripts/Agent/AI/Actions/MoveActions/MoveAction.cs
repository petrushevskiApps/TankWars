using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class MoveAction : GoapAction
{
	protected AIAgent agent;
	protected MemoryController agentMemory;
	protected NavigationController agentNavigation;
	protected bool isActionExited = false;

	private void Start()
	{
		agent = GetComponent<AIAgent>();
		agentMemory = agent.Memory;
		agentNavigation = agent.Navigation;
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