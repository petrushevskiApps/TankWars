
using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class GoapAction : MonoBehaviour 
{
	public string ActionName { get => GetType().ToString(); }

	[SerializeField] protected float minimumCost = 1f;

	public float minRequiredRange = 10f;
	public float maxRequiredRange = 15f;
	public bool requiresRange = false;

	public Dictionary<string, bool> Preconditions { get; private set; }
	public Dictionary<string, bool> Effects { get; private set; }

	
	protected Action actionCompleted;
	protected Action actionFailed;
	protected Action actionReset;

	protected GameObject target;

	private bool inRange = false;

	public bool IsInRange
	{
		get => requiresRange ? inRange : true;
		set => inRange = value;
	}

	// Used by Goap Agent to know if action has target
	public bool IsTargetAcquired => target != null;

	// Used by Goap Agent to know when action is done
	public bool IsActionDone { get; set; } = false;

	// Prevent exiting action multiple times by events
	public bool IsActionExited { get; set; } = false;

	public GoapAction() 
	{
		Preconditions = new Dictionary<string, bool>();
		Effects = new Dictionary<string, bool>();
	}

	 // Reset any variables that need to be reset 
	 // before planning happens again.
	public virtual void ResetAction() 
	{
		target = null;
		IsInRange = false;
		IsActionDone = false;
		IsActionExited = false;
	}

	/* Action Preconditions & Effects */
	public void AddPrecondition(string key, bool value)
	{
		Preconditions.Add(key, value);
	}

	public void AddEffect(string key, bool value)
	{
		Effects.Add(key, value);
	}



	/* Action Lifecycle */
	public abstract float GetCost();
	public abstract bool CheckProceduralPreconditions();

	public abstract void EnterAction(Action Success, Action Fail, Action Reset);

	public abstract void ExecuteAction();

	protected abstract void ExitAction(Action exitAction);

	public abstract void SetActionTarget();

	public abstract void InvalidTargetLocation();

	

}