using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GOAP;

public sealed class GoapAgent : MonoBehaviour 
{
	private const string IDLE_STATE_KEY				= "IdleState";
	private const string MOVETO_STATE_KEY			= "MoveToState";
	private const string PERFORMACTION_STATE_KEY	= "PerformActionState";

	private HashSet<GoapAction> availableActions;
	private Queue<GoapAction> currentActions;

	// this is the implementing class that provides 
	// our world data and listens to feedback on planning
	private IGoap agentImplementation; 

	private GoapPlanner planner;

	private Animator FSM; 

	void Start () 
	{
		availableActions = new HashSet<GoapAction> ();
		currentActions = new Queue<GoapAction> ();
		planner = new GoapPlanner ();

		agentImplementation = GetComponent<IGoap>();
		FSM = GetComponent<Animator>();

		LoadActions ();
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
		// GOAP Planning State

		// get the world state and the goal we want to plan for
		HashSet<KeyValuePair<string, object>> worldState = agentImplementation.GetWorldState();
		HashSet<KeyValuePair<string, object>> goal = agentImplementation.CreateGoalState();

		// Plan
		Queue<GoapAction> plan = planner.Plan(gameObject, availableActions, worldState, goal);

		if (plan != null)
		{
			// we have a plan, hooray!
			currentActions = plan;
			agentImplementation.PlanFound(goal, plan);

			// Plan Available - Change State
			ChangeState(PERFORMACTION_STATE_KEY);

		}
		else
		{
			// ugh, we couldn't get a plan
			Debug.Log("Failed Plan: " + goal);
			agentImplementation.PlanFailed(goal);

			// Plan Not Available - Loop State
			ChangeState(IDLE_STATE_KEY);
		}
	}

	public void MoveToState()
	{
		GoapAction action = currentActions.Peek();

		if (action.RequiresInRange() && action.target == null)
		{
			Debug.Log("Fatal error: Action requires a target but has none. Planning failed. You did not assign the target in your Action.checkProceduralPrecondition()");
			ChangeState(IDLE_STATE_KEY);
			return;
		}

		// get the agent to move itself
		Debug.Log("Move to do: " + action.name);

		if (agentImplementation.MoveAgent(action))
		{
			// Destination Reached - Change State
			ChangeState(PERFORMACTION_STATE_KEY);
		}
		else
		{
			// Destination Not Reached - Loop
			ChangeState(MOVETO_STATE_KEY);
		}
	}


	public void PerformActionState()
	{
		// perform the action

		if (!HasActionPlan())
		{
			// no actions to perform
			Debug.Log("<color=red>Done actions</color>");
			ChangeState(IDLE_STATE_KEY);
			agentImplementation.ActionsFinished();
			return;
		}

		GoapAction action = currentActions.Peek();
		if (action.IsActionDone())
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
					ChangeState(IDLE_STATE_KEY);
					agentImplementation.PlanAborted(action);
				}
				else
				{
					ChangeState(PERFORMACTION_STATE_KEY);
				}
			}
			else
			{
				// we need to move there first
				// push moveTo state
				ChangeState(MOVETO_STATE_KEY);
			}

		}
		else
		{
			// no actions left, move to Plan state
			ChangeState(IDLE_STATE_KEY);
			agentImplementation.ActionsFinished();
		}
	}
	
	private void ChangeState(string stateKey)
	{
		FSM.SetTrigger(stateKey);
	}

}
