using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GOAP;

public sealed class GoapAgent : MonoBehaviour 
{

	private HashSet<GoapAction> availableActions;
	private Queue<GoapAction> currentActions;

	private IGoap dataProvider; // this is the implementing class that provides our world data and listens to feedback on planning

	private GoapPlanner planner;

	[SerializeField] private Animator unityFSM; 

	void Start () 
	{
		availableActions = new HashSet<GoapAction> ();
		currentActions = new Queue<GoapAction> ();
		planner = new GoapPlanner ();

		dataProvider = GetComponent<IGoap>();

		LoadActions ();

		unityFSM = GetComponent<Animator>();

		if (unityFSM.GetCurrentAnimatorStateInfo(0).IsName("MoveToState"))
		{
			Debug.Log("State Entered");
		}
	}
	

	public void AddAction(GoapAction a) 
	{
		availableActions.Add (a);
	}
	public void RemoveAction(GoapAction action)
	{
		availableActions.Remove(action);
	}

	private bool HasActionPlan()
	{
		return currentActions.Count > 0;
	}

	public GoapAction GetAction(Type actionType) 
	{
		foreach (GoapAction action in availableActions) 
		{
			if (action.GetType().Equals(actionType) )
			{
				return action;
			}
		}
		return null;
	}

	private void LoadActions()
	{
		GoapAction[] actions = gameObject.GetComponents<GoapAction>();

		foreach (GoapAction a in actions)
		{
			availableActions.Add(a);
		}
		Debug.Log("Found actions: " + actions.Length);
	}

	public void IdleState()
	{
		// GOAP planning

		// get the world state and the goal we want to plan for
		HashSet<KeyValuePair<string, object>> worldState = dataProvider.GetWorldState();
		HashSet<KeyValuePair<string, object>> goal = dataProvider.CreateGoalState();

		// Plan
		Queue<GoapAction> plan = planner.Plan(gameObject, availableActions, worldState, goal);

		if (plan != null)
		{
			// we have a plan, hooray!
			currentActions = plan;
			dataProvider.PlanFound(goal, plan);

			unityFSM.SetTrigger("PerformActionState");

		}
		else
		{
			// ugh, we couldn't get a plan
			Debug.Log("Failed Plan: " + goal);
			dataProvider.PlanFailed(goal);
			unityFSM.SetTrigger("IdleState");
			// Stays in IdleState;

		}
	}

	public void MoveToState()
	{
		GoapAction action = currentActions.Peek();

		if (action.RequiresInRange() && action.target == null)
		{
			Debug.Log("Fatal error: Action requires a target but has none. Planning failed. You did not assign the target in your Action.checkProceduralPrecondition()");
			unityFSM.SetTrigger("IdleState");
			return;
		}

		// get the agent to move itself
		Debug.Log("Move to do: " + action.name);
		if (dataProvider.MoveAgent(action))
		{
			unityFSM.SetTrigger("PerformActionState");
		}
		else
		{
			unityFSM.SetTrigger("MoveToState");
		}
	}


	public void PerformActionState()
	{
		// perform the action

		if (!HasActionPlan())
		{
			// no actions to perform
			Debug.Log("<color=red>Done actions</color>");
			unityFSM.SetTrigger("IdleState");
			dataProvider.ActionsFinished();
			return;
		}

		GoapAction action = currentActions.Peek();
		if (action.isDone())
		{
			// the action is done. Remove it so we can perform the next one
			currentActions.Dequeue();
		}

		if (HasActionPlan())
		{
			// perform the next action
			action = currentActions.Peek();
			bool inRange = action.RequiresInRange() ? action.IsInRange() : true;

			if (inRange)
			{
				// we are in range, so perform the action
				bool success = action.Perform(gameObject);

				if (!success)
				{
					// action failed, we need to plan again
					unityFSM.SetTrigger("IdleState");
					dataProvider.PlanAborted(action);
				}
				else
				{
					unityFSM.SetTrigger("PerformActionState");

				}
			}
			else
			{
				// we need to move there first
				// push moveTo state
				unityFSM.SetTrigger("MoveToState");
			}

		}
		else
		{
			// no actions left, move to Plan state
			unityFSM.SetTrigger("IdleState");
			dataProvider.ActionsFinished();
		}
	}
	
	

}
