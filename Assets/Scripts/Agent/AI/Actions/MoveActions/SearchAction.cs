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

	public override void ExecuteAction()
	{
		RestartAction();
	}
}
