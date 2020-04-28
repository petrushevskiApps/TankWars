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

	public void IdleState()
	{
		// GOAP Planning State

		// get the world state and the goal we want to plan for
		Dictionary<string, bool> worldState = agentImplementation.GetWorldState();
		Dictionary<string, bool> goal = agentImplementation.CreateGoalState();


		// Plan
		Queue<GoapAction> plan = planner.Plan(gameObject, availableActions, worldState, goal);

		if (plan != null)
		{
			// we have a plan, hooray!
			currentActions = plan;
			agentImplementation.PlanFound(goal, plan);

			// Plan Available - Change State
			ChangeState(FSMKeys.PERFORM_STATE);

		}
		else
		{
			// ugh, we couldn't get a plan
			Debug.Log("Failed Plan: " + goal);
			agentImplementation.PlanFailed(goal);

			// Plan Not Available - Loop State
			ChangeState(FSMKeys.IDLE_STATE);
		}
	}

	// Called from FSM
	public void MoveToState()
	{
		GoapAction action = currentActions.Peek();

		if (action.RequiresInRange())
		{
			bool targetAcquired = action.SetActionTarget();

			if(!targetAcquired)
			{
				Debug.LogError($"Fatal error:{gameObject.name} | {action.name} requires a target but has none. Planning failed.");
				ChangeState(FSMKeys.IDLE_STATE);
				return;
			}
		}

		if(!action.CheckProceduralPrecondition(gameObject))
		{
			ChangeState(FSMKeys.IDLE_STATE);
		}

		// Get the agent to move itself
		if (agentImplementation.MoveAgent(action))
		{
			// Destination Reached - Change State
			ChangeState(FSMKeys.PERFORM_STATE);
		}
		else
		{
			// Destination Not Reached - Loop
			ChangeState(FSMKeys.MOVETO_STATE);
		}


	}


	public void PerformActionState()
	{
		// perform the action

		if (!HasActionPlan())
		{
			// no actions to perform
			Debug.Log("<color=red>Done actions</color>");
			ChangeState(FSMKeys.IDLE_STATE);
			agentImplementation.ActionsFinished();
			return;
		}

		currentAction = currentActions.Peek();
		Debug.Log($"<color=green> {gameObject.name} Perform Action: {currentAction.name}</color>");

		if (currentAction.IsActionDone())
		{
			// the action is done. Remove it so we can perform the next one
			currentActions.Dequeue();
		}

		if (HasActionPlan())
		{
			// perform the next action
			currentAction = currentActions.Peek();
			bool inRange  = currentAction.RequiresInRange() ? currentAction.IsInRange() : true;

			if (inRange)
			{
				// we are in range, so perform the action
				currentAction.Perform(gameObject, 
					()=> ChangeState(FSMKeys.PERFORM_STATE),
					()=>{
						// action failed, we need to plan again
						ChangeState(FSMKeys.IDLE_STATE);
						agentImplementation.PlanAborted(currentAction);
					});
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
	
	private void ChangeState(string stateKey)
	{
		FSM.SetTrigger(stateKey);
	}

}
