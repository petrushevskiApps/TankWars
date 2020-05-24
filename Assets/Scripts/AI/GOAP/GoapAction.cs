
using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public abstract class GoapAction : MonoBehaviour 
{
	[HideInInspector] public new string actionName = "No Name";
	

	public float minRequiredRange = 10f;
	public float maxRequiredRange = 15f;
	public bool requiresRange = false;

	public Dictionary<string, bool> Preconditions { get; }
	public Dictionary<string, bool> Effects { get; }

	[SerializeField] protected float cost = 1f;
	
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

	public bool IsActionDone { get; set; } = false;

	public GoapAction() 
	{
		Preconditions = new Dictionary<string, bool>();
		Effects = new Dictionary<string, bool>();
	}

	/**
	 * Reset any variables that need to be reset 
	 * before planning happens again.
	 */
	public virtual void ResetAction() 
	{
		target = null;
		IsInRange = false;
		IsActionDone = false;
	}

	public abstract void SetActionTarget();

	public abstract void InvalidTargetLocation();

	public bool IsTargetAcquired()
	{
		return target != null;
	}

	public virtual float GetCost()
	{
		return cost;
	}
	public GameObject GetTarget()
	{
		return target;
	}
	/* Action Lifecycle */
	public abstract bool TestProceduralPreconditions();

	public abstract void EnterAction(Action Success, Action Fail, Action Reset);

	public abstract void ExecuteAction(GameObject agent);

	protected abstract void ExitAction(Action exitAction);

	/* Action Preconditions & Effects */
	public void AddPrecondition(string key, bool value)  
	{
		Preconditions.Add (key, value);
	}

	public void RemovePrecondition(string key)  
	{
		try
		{
			Preconditions.Remove(key);
		}
		catch (Exception e)
		{
			Debug.LogError($"Key: {key} not found in Preconditions dictionary");
		}
	}

	public void AddEffect(string key, bool value)  
	{
		Effects.Add (key, value);
	}

	public void RemoveEffect(string key)  
	{
		try
		{
			Effects.Remove(key);
		}
		catch(Exception e)
		{
			Debug.LogError($"Key: {key} not found in Effects dictionary");
		}
	}

	
}