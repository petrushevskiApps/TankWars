using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GOAP
{
	//using State = KeyValuePair<string, bool>; 
	//using Goals = Dictionary<string, bool>;
	//using GoapActions = HashSet<GoapAction>;

	/**
	 * Plans what actions can be completed in order to fulfill a goal state.
	 */
	public class GoapPlanner
	{
		
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

			// build graph
			Node start = new Node(null, 0, worldState, null);
			bool success = BuildTree(start, leaves, usableActions, goals);
			

			if (!success)
			{
				Debug.Log("NO PLAN");
				return null;
			}
			else
			{
				Utilities.PrintTree(agent.name, leaves, GetCheapestLeaf(leaves));

				Queue<GoapAction> actionQueue = new Queue<GoapAction>();

				GetActionList(leaves).ForEach(x => actionQueue.Enqueue(x));

				return actionQueue;
			}

		}

		private void ResetActions(HashSet<GoapAction> availableActions)
		{
			foreach (GoapAction action in availableActions)
			{
				action.ResetAction();
			}
		}
		
		private Node GetCheapestLeaf(List<Node> leaves)
		{
			Node result = null;

			foreach (Node leaf in leaves)
			{
				if (result == null)
				{
					result = leaf;
				}
				else if (leaf.runningCost < result.runningCost)
				{
					result = leaf;
				}
			}

			return result;
		}

		private List<GoapAction> GetActionList(List<Node> leaves)
		{
			List<GoapAction> result = new List<GoapAction>();

			Node node = GetCheapestLeaf(leaves);

			while (node != null)
			{
				if (node.action != null)
				{
					// insert the action in the front
					result.Insert(0, node.action);
				}
				node = node.parent;
			}

			Utilities.PrintCollection("Actions", result);

			return result;
		}
		
		/**
		 * Returns true if at least one solution was found.
		 * The possible paths are stored in the leaves list. Each leaf has a
		 * 'runningCost' value where the lowest cost will be the best action
		 * sequence.
		 */
		private bool BuildTree(Node parent, List<Node> leaves, HashSet<GoapAction> usableActions, Dictionary<string, bool> goal)
		{
			bool foundOne = false;

			Utilities.PrintCollection("Usable Actions", usableActions);

			// Go through each action available at this node
			// and see if we can use it here
			foreach (GoapAction action in usableActions)
			{
				
				// if the parent state has the conditions for this
				// action's preconditions, we can use it here
				if (InState(action.Preconditions, parent.state))
				{

					// apply the action's effects to the parent state
					Dictionary<string, bool> currentState = UpdatedState(parent.state, action.Effects);

					Node node = new Node(parent, parent.runningCost + action.cost, currentState, action);

					// If the current state has the conditions for achieving
					// agents goal, we have found one possible plan.
					if (CheckGoal(goal, currentState))
					{
						leaves.Add(node);
						foundOne = true;
					}
					else
					{
						// not at a solution yet, so test all the remaining actions and branch out the tree
						HashSet<GoapAction> actionsSubset = GetActionsSubset(usableActions, action);
						Utilities.PrintCollection("Usable Actions Subset", actionsSubset);
						bool found = BuildTree(node, leaves, actionsSubset, goal);
						
						if (found)
						{
							foundOne = true; 
						}
					}
				}
			}

			return foundOne;
		}

		/**
		 * Create a subset of the actions excluding 
		 * the used action = removeAction. 
		 * Creates a new set.
		 */
		private HashSet<GoapAction> GetActionsSubset(HashSet<GoapAction> actions, GoapAction removeAction)
		{
			HashSet<GoapAction> subset = new HashSet<GoapAction>();

			foreach (GoapAction a in actions)
			{
				if (!a.Equals(removeAction))
				{
					subset.Add(a);
				}
			}

			return subset;
		}

		/**
		 * Check that all items in 'test' are in 'state'.
		 * If just one does not match or is not there
		 * then this returns false.
		 */
		private bool InState(Dictionary<string, bool> test, Dictionary<string, bool> states)
		{
			foreach (KeyValuePair<string, bool> testState in test) 
			{

				if(!states.ContainsKey(testState.Key))
				{
					return false;
				}
				else
				{
					if(states[testState.Key] != testState.Value)
					{
						return false;
					}
				}
			}
			return true;
		}
		private bool CheckGoal(Dictionary<string, bool> goals, Dictionary<string, bool> state)
		{
			foreach (KeyValuePair<string, bool> goal in goals)
			{
				if(state.ContainsKey(goal.Key))
				{
					return true;
				}
			}
			return false;
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




