using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GOAP;

public sealed class GoapAgent : MonoBehaviour 
{
	private HashSet<GoapAction> availableActions;
	private Queue<GoapAction> currentActions;

	// this is the implementing class that provides 
	// our world data and listens to feedback on planning
	private IGoap agentImplementation; 

	private GoapPlanner planner;

	private Animator FSM;

	private GoapAction currentAction;
	
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

	// Collect all available actions for the agent
	private void LoadActions()
	{
		GoapAction[] actions = gameObject.GetComponents<GoapAction>();

		foreach (GoapAction a in actions)
		{
			availableActions.Add(a);
		}
		Debug.Log("Found actions: " + actions.Length);
	}

	int goalIndex = 0;

	public void IdleState()
	{
		// GOAP Planning State

		// get the world state and the goal we want to plan for
		Dictionary<string, bool> worldState = agentImplementation.GetWorldState();
		Dictionary<string, bool> goal = agentImplementation.GetGoalState(goalIndex);


		agentImplementation.ShowMessage("Planning...");
		// Plan
		Queue<GoapAction> plan = planner.Plan(gameObject, availableActions, worldState, goal);

		if (plan != null)
		{
			// we have a plan, hooray!
			currentActions = plan;
			agentImplementation.PlanFound(goal, plan);
			goalIndex = 0;

			// Plan Available - Change State
			ChangeState(FSMKeys.PERFORM_STATE);
		}
		else
		{
			// ugh, we couldn't get a plan
			Debug.Log("Failed Plan: " + goal);
			agentImplementation.PlanFailed(goal);
			
			if (goalIndex < agentImplementation.GetGoalsCount() - 1)
			{
				goalIndex++;
			}
			else
			{
				goalIndex = 0;
			}
			// Plan Not Available - Loop State
			ChangeState(FSMKeys.IDLE_STATE);
		}
	}

	// Called from FSM
	public void MoveToState()
	{
		GoapAction action = currentActions.Peek();

		bool conditionsMeet = action.CheckPreconditions(gameObject);

		if (!conditionsMeet)
		{
			// Plan Failed - RePlan
			ChangeState(FSMKeys.IDLE_STATE);
		}
		else if(InRange())
		{
			// Destination Reached - Change State
			ChangeState(FSMKeys.PERFORM_STATE);
		}
		else
		{
			action.SetActionTarget();

			if (!action.IsTargetAcquired())
			{
				Debug.LogError($"Fatal error:{gameObject.name} | {action.name} target missing!");
				ChangeState(FSMKeys.IDLE_STATE);
			}
			else
			{
				// Get the agent to move itself
				agentImplementation.ShowMessage("Going to " + action.name);
				agentImplementation.MoveAgent(action);

				// Destination Not Reached - Loop
				ChangeState(FSMKeys.MOVETO_STATE);
			}
		}
	}


	public void PerformActionState()
	{
		if (HasActionPlan())
		{
			// perform the next action
			currentAction = currentActions.Peek();

			if (InRange())
			{
				agentImplementation.ShowMessage(currentAction.name);
				// we are in range, so perform the action
				currentAction.ExecuteAction(gameObject, OnActionSuccess, OnActionFail);
			}
			else
			{
				// we need to move there first
				// push moveTo state
				ChangeState(FSMKeys.MOVETO_STATE);
			}

		}
		else
		{
			// no actions left, move to Plan state
			ChangeState(FSMKeys.IDLE_STATE);
			agentImplementation.ActionsFinished();
		}
	}

	private bool InRange()
	{
		return currentAction.RequiresInRange() ? currentAction.IsInRange() : true;
	}

	private void OnActionSuccess()
	{
		if (currentAction.IsActionDone())
		{
			// the action is done. Remove it so we can perform the next one
			currentActions.Dequeue();
		}
		ChangeState(FSMKeys.PERFORM_STATE);
	}

	private void OnActionFail() 
	{
		// action failed, we need to plan again
		ChangeState(FSMKeys.IDLE_STATE);
		agentImplementation.PlanAborted(currentAction);
	}

	private void ChangeState(string stateKey)
	{
		FSM.SetTrigger(stateKey);
	}

}
