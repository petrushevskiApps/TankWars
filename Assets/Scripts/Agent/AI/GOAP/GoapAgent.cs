using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GOAP;
using System.Linq;

public sealed class GoapAgent : MonoBehaviour 
{
	public AgentState State { get; private set; }
	public string CurrentPlanTextual { get; private set; } = "No Plan";

	private HashSet<GoapAction> availableActions;
	private Queue<GoapAction> currentPlan;

	// this is the implementing class that provides 
	// our world data and listens to feedback on planning
	private IGoap agentImplementation; 

	private GoapPlanner planner;

	private Animator FSM;

	private GoapAction currentAction;

	private int goalIndex = 0;

	void Awake () 
	{
		availableActions = new HashSet<GoapAction> ();
		currentPlan = new Queue<GoapAction> ();
		planner = new GoapPlanner ();

		agentImplementation = GetComponent<IGoap>();
		FSM = GetComponent<Animator>();

		LoadActions ();
	}

	private bool HasActionPlan()
	{
		return currentPlan.Count > 0;
	}

	// Collect all available actions for the agent
	private void LoadActions()
	{
		GoapAction[] actions = gameObject.GetComponents<GoapAction>();

		foreach (GoapAction a in actions)
		{
			availableActions.Add(a);
		}
	}

	// GOAP Planning State
	public void IdleState()
	{
		State = AgentState.Planning;
		
		// Get the world state and the goal we want to plan for
		Dictionary<string, bool> worldState = agentImplementation.GetWorldState();
		Dictionary<string, bool> goal = agentImplementation.GetGoalState(goalIndex);


		// Clear previous plan ( actions )
		currentPlan.Clear();
		
		// Find new plan
		currentPlan = planner.Plan(gameObject, availableActions, worldState, goal, goalIndex == 0 ? "Eliminate" : "Find Enemy");


		if (HasActionPlan())
		{
			CurrentPlanTextual = Utilities.GetCollectionString(currentPlan.ToList());
			
			goalIndex = 0;

			// Plan Available - Change State
			ChangeState(FSMKeys.PERFORM_STATE);
		}
		else
		{
			
			Debug.LogError("IdleState: Failed Plan | Goal Index:: " + goalIndex + "\n");

			// Loop between goal index 0 and maximum number of goals
			goalIndex = (int) Mathf.Repeat(++goalIndex, agentImplementation.GetGoalsCount());

			// Plan Not Available - Loop State
			ChangeState(FSMKeys.IDLE_STATE);
		}
	}

	public void MoveToState()
	{
		State = AgentState.Moving;

		if (!HasActionPlan()) return;

		if (!currentAction.IsTargetAcquired)
		{
			Debug.LogError("Action Failed: " + currentAction.ActionName + " Error: Target not accquired!");
			ChangeState(FSMKeys.IDLE_STATE);
		}
		else
		{
			if (currentAction.IsInRange)
			{
				// Destination Reached - Change State
				ChangeState(FSMKeys.PERFORM_STATE);
			}
			else
			{
				// Get the agent to move itself
				agentImplementation.MoveAgent(currentAction);

				// Destination Not Reached - Loop
				ChangeState(FSMKeys.MOVETO_STATE);
			}

		}

	}

	public void PerformActionState()
	{
		State = AgentState.ExecutingAction;

		if (HasActionPlan())
		{
			// perform the next action
			currentAction = currentPlan.Peek();
			currentAction.EnterAction(OnActionSuccess, OnActionFail, OnActionReset);
			
			if (currentAction.IsInRange)
			{
				// we are in range, so perform the action
				currentAction.ExecuteAction();
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
		}
	}

	private void OnActionSuccess()
	{		
		if (currentAction.IsActionDone)
		{
			// the action is done. Remove it so we can perform the next one
			currentPlan.Dequeue();
			
			if (!HasActionPlan())
			{
				ChangeState(FSMKeys.IDLE_STATE);
				return;
			}
			
		}

		ChangeState(FSMKeys.PERFORM_STATE);
	}

	private void OnActionFail() 
	{
		// Action failed, we need to plan again
		ChangeState(FSMKeys.IDLE_STATE);
	}
	
	private void OnActionReset()
	{
		// Action needs to be restarted
		ChangeState(FSMKeys.PERFORM_STATE);
	}
	
	private void ChangeState(string stateKey)
	{
		FSM.SetTrigger(stateKey);
	}

	public string GetCurrentAction()
	{
		if(currentAction != null)
		{
			return currentAction.ActionName;
		}
		return "No Action Selected";
	}

	public void AddAction(GoapAction a)
	{
		availableActions.Add(a);
	}
	public void RemoveAction(GoapAction action)
	{
		availableActions.Remove(action);
	}
}
