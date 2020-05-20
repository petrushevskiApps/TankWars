using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GOAP;
using System.Linq;
using System.Text;

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
	
	int goalIndex = 0;

	private StringBuilder breadCrumbs = new StringBuilder();

	public AgentState State { get; private set; }
	public String currentPlan = "No Plan";

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
		breadCrumbs.Clear();
		breadCrumbs.Append("Idle State: Enter\n");

		State = AgentState.Planning;
		// GOAP Planning State

		// get the world state and the goal we want to plan for
		Dictionary<string, bool> worldState = agentImplementation.GetWorldState();
		Dictionary<string, bool> goal = agentImplementation.GetGoalState(goalIndex);


		agentImplementation.ShowMessage("Planning...");

		breadCrumbs.Append("Idle State: Before Plan: " + currentActions.Count + "\n");
		// Plan
		Queue<GoapAction> plan = planner.Plan(gameObject, availableActions, worldState, goal);

		breadCrumbs.Append("Idle State: After Plan: " + currentActions.Count + "\n");

		if (plan.Count > 0)
		{
			breadCrumbs.Append("Idle State: Plan Found: " + plan.Count + "\n");

			currentPlan = Utilities.GetCollectionString(plan.ToList());
			// we have a plan, hooray!
			currentActions = plan;
			agentImplementation.PlanFound(goal, plan);
			goalIndex = 0;

			breadCrumbs.Append("Idle State: Go To Perform State: " + currentActions.Count + "\n");
			// Plan Available - Change State
			ChangeState(FSMKeys.PERFORM_STATE);
		}
		else
		{
			breadCrumbs.Append("Idle State: Plan not found!! \n");
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

			breadCrumbs.Append("Idle State: Loop to IDLE State!! Goal Index: " + goalIndex +" \n");
			// Plan Not Available - Loop State
			ChangeState(FSMKeys.IDLE_STATE);
		}

		breadCrumbs.Append("Idle State: Exit\n");
	}

	public void MoveToState()
	{
		breadCrumbs.Append("MoveTo State: Enter\n");

		State = AgentState.Moving;

		if (currentActions.Count <= 0)
		{
			breadCrumbs.Append("MoveTo State: Error:\n");
			Debug.LogError(breadCrumbs.ToString());

			return;
		}

		GoapAction action = currentActions.Peek();

		breadCrumbs.Append("MoveTo State: Action:" + action.actionName + "\n");

		bool conditionsMeet = action.CheckPreconditions(gameObject);

		if (!conditionsMeet)
		{
			breadCrumbs.Append("MoveTo State: Conditions Failed, Go to IDLE State!!\n");
			// Plan Failed - RePlan
			ChangeState(FSMKeys.IDLE_STATE);
		}
		else if(currentAction.IsInRange)
		{
			breadCrumbs.Append("MoveTo State: Action in range, Go to PERFORM State!!\n");
			// Destination Reached - Change State
			ChangeState(FSMKeys.PERFORM_STATE);
		}
		else
		{
			breadCrumbs.Append("MoveTo State: Moving Start!!\n");

			if (!action.IsTargetAcquired())
			{
				Debug.LogError($"Fatal error:{gameObject.name} | {action.name} target missing!");
				breadCrumbs.Append("MoveTo State: Target not Acquired, Go to IDLE State!!\n");
				ChangeState(FSMKeys.IDLE_STATE);
			}
			else
			{
				// Get the agent to move itself
				breadCrumbs.Append("MoveTo State: Moving Agent!!\n");
				agentImplementation.ShowMessage("Going to " + action.name);
				agentImplementation.MoveAgent(action);

				breadCrumbs.Append("MoveTo State: Loop MOVE_TO State!!\n");
				// Destination Not Reached - Loop
				ChangeState(FSMKeys.MOVETO_STATE);
			}

			breadCrumbs.Append("MoveTo State: Moving End!!\n");
		}

		breadCrumbs.Append("MoveTo State: Exit\n");
	}


	public void PerformActionState()
	{
		breadCrumbs.Append("Perform State: Enter\n");

		State = AgentState.ExecutingAction;

		if (HasActionPlan())
		{
			breadCrumbs.Append("Perform State: Has Action Plan Count: " + currentActions.Count + "\n");
			// perform the next action
			currentAction = currentActions.Peek();
			breadCrumbs.Append("Perform State: Enter Action\n");
			currentAction.EnterAction(OnActionSuccess, OnActionFail);
			
			if (currentAction.IsInRange)
			{
				breadCrumbs.Append("Perform State: Action In Range, Execute\n");
				agentImplementation.ShowMessage(currentAction.name);
				// we are in range, so perform the action
				currentAction.ExecuteAction(gameObject);
			}
			else
			{
				breadCrumbs.Append("Perform State: Action NOT In Range, Go to MoveTo State!!\n");
				// we need to move there first
				// push moveTo state
				ChangeState(FSMKeys.MOVETO_STATE);
			}

		}
		else
		{
			breadCrumbs.Append("Perform State: No Actions Left, Go to IDLE State!!\n");
			// no actions left, move to Plan state
			ChangeState(FSMKeys.IDLE_STATE);
			agentImplementation.ActionsFinished();
		}
	}

	private void OnActionSuccess()
	{
		breadCrumbs.Append("OnActionSuccess: Action::" + currentAction.actionName + "\n");
		
		if (currentAction.IsActionDone)
		{
			// the action is done. Remove it so we can perform the next one
			currentActions.Dequeue();
			
			if (currentActions.Count <= 0)
			{
				breadCrumbs.Append("OnActionSuccess: Actions Count::" + currentActions.Count + " Go to IDLE State!!\n");
				ChangeState(FSMKeys.IDLE_STATE);
				return;
			}
			breadCrumbs.Append("OnActionSuccess: Actions Count::" + currentActions.Count + "\n");
		}
		breadCrumbs.Append("OnActionSuccess: Go to Perform State!!\n");
		ChangeState(FSMKeys.PERFORM_STATE);
	}

	private void OnActionFail() 
	{
		breadCrumbs.Append("OnActionFail: Go to IDLE State!!\n");
		// action failed, we need to plan again
		ChangeState(FSMKeys.IDLE_STATE);
		agentImplementation.PlanAborted(currentAction);
	}

	private void ChangeState(string stateKey)
	{
		breadCrumbs.Append("ChangeState: State Key:: " + stateKey + "\n");
		FSM.SetTrigger(stateKey);
	}

	public string GetCurrentAction()
	{
		if(currentAction != null)
		{
			return currentAction.GetType().ToString();
		}
		return "No Action Selected";
	}

	public string GetCurrentPlanString()
	{
		return currentPlan;
	}
}
