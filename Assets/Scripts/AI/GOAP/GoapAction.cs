
using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public abstract class GoapAction : MonoBehaviour 
{

	public string name = "No Name";
	private bool inRange = false;
	public bool requireAngle = false;

	public float minRequiredRange = 10f;
	public float maxRequiredRange = 15f;

	public Dictionary<string, bool> Preconditions { get; }
	public Dictionary<string, bool> Effects { get; }

	/* The cost of performing the action. 
	 * Figure out a weight that suits the action. 
	 * Changing it will affect what actions are chosen during planning.*/
	public float cost = 1f;

	/**
	 * An action often has to perform on an object. This is that object. Can be null. */
	public GameObject target;

	public GoapAction() 
	{
		Preconditions = new Dictionary<string, bool>();
		Effects = new Dictionary<string, bool>();
		//target = target;
	}

	public void ResetAction() 
	{
		inRange = false;
		Reset ();
	}

	/**
	 * Reset any variables that need to be reset before planning happens again.
	 */
	public abstract void Reset();

	/**
	 * Is the action done?
	 */
	public abstract bool IsActionDone();

	/**
	 * Procedurally check if this action can run. Not all actions
	 * will need this, but some might.
	 */
	public abstract bool CheckProceduralPrecondition(GameObject agent);

	/**
	 * Run the action.
	 * Returns True if the action performed successfully or false
	 * if something happened and it can no longer perform. In this case
	 * the action queue should clear out and the goal cannot be reached.
	 */
	public abstract void Perform(GameObject agent, Action success, Action fail);

	/**
	 * Does this action need to be within range of a target game object?
	 * If not then the moveTo state will not need to run for this action.
	 */
	public abstract bool RequiresInRange ();
	
	/**
	 * Are we in range of the target?
	 * The MoveTo state will set this and it gets reset each time this action is performed.
	 */
	public bool IsInRange ()  
	{
		return inRange;
	}
	
	public void SetInRange(bool inRange) 
	{
		this.inRange = inRange;
	}


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