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
		public Queue<GoapAction> Plan(GameObject agent, HashSet<GoapAction> availableActions, Dictionary<string, bool> worldState, Dictionary<string, bool> goals) 
		{
			// reset the actions so we can start fresh with them
			ResetActions(availableActions);

			// check what actions can run using their checkProceduralPrecondition
			HashSet<GoapAction> usableActions = new HashSet<GoapAction>();

			foreach (GoapAction action in availableActions)
			{
				usableActions.Add(action);
			}

			// we now have all actions that can run, stored in usableActions
			// build up the tree and record the leaf nodes that provide a solution to the goal.
			List<Node> leaves = new List<Node>();

			leaves = FindPlan(usableActions, worldState, goals);
			
			if (leaves.Count == 0)
			{
				Debug.Log("NO PLAN");
				return null;
			}
			else
			{
				Utilities.PrintGOAPPlan(agent.name, leaves);

				Queue<GoapAction> actionQueue = new Queue<GoapAction>();

				leaves.ForEach(x => { if (x.action != null) actionQueue.Enqueue(x.action); });

				return actionQueue;
			}

		}

		

		private List<Node> FindPlan(HashSet<GoapAction> availableActions, Dictionary<string, bool> worldState, Dictionary<string, bool> goal)
		{
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

				// Remove used action
				availableActions.Remove(currentState.action);

				// remove it from the open list
				openListStates.Remove(currentState);

				// if we added the destination to the closed list, we've found a path
				if (InState(goal, currentState.state))
				{
					break;
				}

				// Retrieve all states next possible
				adjacentStates = GetAdjacentStates(currentState, availableActions, goal);

				foreach(Node state in adjacentStates)
				{
					// if this adjacent square is already in the closed list ignore it
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
						// test if using the current G score make the aSquare F score lower,
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
			return closedListStates;
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
				if (InState(action.Preconditions, parent.state))
				{
					// Check if the Procedural Preconditions
					// are also matched ( These are usually OR conditions )
					if(action.CheckProceduralPreconditions())
					{
						// apply the action's effects to the parent state
						Dictionary<string, bool> state = UpdatedState(parent.state, action.Effects);

						// Create new Node State
						Node node = new Node(parent, parent.runningCost + action.Cost, state, action, goal);

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
}




