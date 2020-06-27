using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;

namespace GOAP
{

	/**
	 * Plans what actions can be completed in order to fulfill a goal state.
	 */
	public class GoapPlanner
	{
		private List<Node> openListStates;
		private List<Node> closedListStates;
		private List<Node> adjacentStates;
		
		/**
		 * Plan what sequence of actions can fulfill the goal.
		 * Returns null if a plan could not be found, or a list of the actions
		 * that must be performed, in order, to fulfill the goal.
		 */
		public Queue<GoapAction> Plan(GameObject agent, HashSet<GoapAction> availableActions, Dictionary<string, bool> worldState, Dictionary<string, bool> goals, string planName) 
		{
			// reset the actions so we can start fresh with them
			ResetActions(availableActions);

			// check what actions can run using their checkProceduralPrecondition
			HashSet<GoapAction> usableActions = new HashSet<GoapAction>();

			foreach (GoapAction action in availableActions)
			{
				usableActions.Add(action);
			}

			Plan plan = FindPlan(usableActions, worldState, goals, agent.name, planName);
			Debug.Log(plan);

			if (!plan.isPlanSuccessful)
			{
				return new Queue<GoapAction>();
			}
			else
			{
				return plan.GetPlanActions();
			}

		}

		

		private Plan FindPlan(HashSet<GoapAction> availableActions, Dictionary<string, bool> worldState, Dictionary<string, bool> goal, string agentName, string planName)
		{
			bool isPlanFound = false;

			openListStates = new List<Node>();
			closedListStates = new List<Node>();
			adjacentStates = new List<Node>();

			// start by adding the original state to the open list
			AddToListAndSort(openListStates, new Node(null, 0, worldState, null, goal));

			while (openListStates.Count > 0)
			{
				//TODO: Open List should be Sorted ( Ascending order by F )
				// Get the square with the lowest F score
				Node currentState = openListStates[0];

				// add the current state to the closed list
				closedListStates.Add(currentState);

				// if we added the destination to the closed list, we've found a path
				if (InState(goal, currentState.State))
				{
					isPlanFound = true;
					break;
				}

				// Remove used action
				availableActions.Remove(currentState.action);

				// remove it from the open list
				//openListStates.Remove(currentState);
				openListStates.Clear();
				// Retrieve all states next possible
				adjacentStates = GetAdjacentStates(currentState, availableActions, goal);

				foreach(Node state in adjacentStates)
				{
					// if this adjacent state is already in the closed list ignore it
					if (closedListStates.Contains(state))
					{
						continue;
					}
					// if its not in the open list
					if(!openListStates.Contains(state))
					{
						// compute its score, set the parent
						AddToListAndSort(openListStates, state);
					}
					// if its already in the open list
					else
					{
						// test if using the current G score make the states F score lower,
						// if yes update the parent because it means its a better path
						int index = openListStates.IndexOf(state);
						Node oldState = openListStates[index];
						
						if(state.StateCost < oldState.StateCost)
						{
							openListStates.RemoveAt(index);
							AddToListAndSort(openListStates, state);
						}
						
					}
				}

			}
			return new Plan(closedListStates, isPlanFound, agentName, planName);
		}
		
		private List<Node> GetAdjacentStates(Node parent, HashSet<GoapAction> availableActions, Dictionary<string, bool> goal)
		{
			List<Node> adjacent = new List<Node>();

			// Go through each action available at this node
			// and see if we can use it here
			foreach (GoapAction action in availableActions)
			{
				// if the parent state has the conditions for this
				// action's preconditions, we can use it here
				if (InState(action.Preconditions, parent.State))
				{
					// Check if the Procedural Preconditions
					// are also matched ( These are OR conditions )
					if(action.CheckProceduralPreconditions())
					{
						// apply the action's effects to the parent state
						Dictionary<string, bool> state = UpdatedState(parent.State, action.Effects);

						// Create new Node State
						Node node = new Node(parent, parent.RunningCost + action.GetCost(), state, action, goal);

						// Add the new state to adjacent states
						AddToListAndSort(adjacent, node);
					}
				}
			}

			return adjacent;
		}


		private void AddToListAndSort(List<Node> list, Node element)
		{
			list.Insert(0, element);
			list.Sort();
		}

		private void ResetActions(HashSet<GoapAction> availableActions)
		{
			foreach (GoapAction action in availableActions)
			{
				action.ResetAction();
			}
		}
		
		
		/**
		 * Check that all items in 'test' are in 'state'.
		 * If just one does not match or is not there
		 * then this returns false.
		 */
		private bool InState(Dictionary<string, bool> test, Dictionary<string, bool> state)
		{
			foreach (KeyValuePair<string, bool> testState in test) 
			{
				if(!state.ContainsKey(testState.Key))
				{
					return false;
				}
				else
				{
					if(state[testState.Key] != testState.Value)
					{
						return false;
					}
				}
			}
			return true;
		}


		/**
		 * Apply the stateChange to the currentState.
		 * If state exists with wrong value - Update value
		 * If state doesn't exist - Add state
		 */
		private Dictionary<string, bool> UpdatedState(Dictionary<string, bool> currentState, Dictionary<string, bool> stateChange)
		{
			Utilities.PrintCollection("Current State", currentState);

			//Make Copy of Current State -> Updated State
			Dictionary<string, bool> updatedState = new Dictionary<string, bool>();

			foreach (KeyValuePair<string, bool> state in currentState)
			{
				updatedState.Add(state.Key, state.Value);
			}


			foreach (KeyValuePair<string,bool> change in stateChange)
			{
				if(updatedState.ContainsKey(change.Key))
				{
					// Update value in updatedState
					updatedState[change.Key] = change.Value;
				}
				else
				{
					// Add state
					updatedState.Add(change.Key, change.Value);
				}
			}

			Utilities.PrintCollection("Updated State", updatedState);
			
			return updatedState;
		}

	}

	public class Plan
	{
		string agentName = "NoName";
		string planName = "NoName";

		public List<Node> plan = new List<Node>();
		public bool isPlanSuccessful = false;

		public Plan(List<Node> plan, bool isPlanSuccessful, string agentName, string planName)
		{
			this.plan = plan;
			this.planName = planName;
			this.isPlanSuccessful = isPlanSuccessful;
			this.agentName = agentName;
		}

		public Queue<GoapAction> GetPlanActions()
		{
			Queue<GoapAction> actionQueue = new Queue<GoapAction>();

			plan.ForEach(x => { if (x.action != null) actionQueue.Enqueue(x.action); });

			return actionQueue;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(isPlanSuccessful ? "<color=green>Successful" : "<color=red>Failed");
			sb.Append(" Plan Name: " + planName + "</color>");
			sb.AppendLine();
			sb.Append(agentName);
			sb.AppendLine();
			sb.Append("GOAP Tree:");
			sb.AppendLine();

			foreach (Node node in plan)
			{
				sb.Append(node.ToString());
				sb.Append("->");
			}
			sb.Append("Goal");

			return sb.ToString();
		}

	}
}




