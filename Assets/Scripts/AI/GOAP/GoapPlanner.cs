using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GOAP
{
	using KeyValue = KeyValuePair<string, object>;
	using StatesSet = HashSet<KeyValuePair<string, object>>;
	using Goals = HashSet<KeyValuePair<string, object>>;
	using GoapActions = HashSet<GoapAction>;

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
		public Queue<GoapAction> Plan(GameObject agent, GoapActions availableActions, StatesSet worldState, Goals goal)
		{
			// reset the actions so we can start fresh with them
			ResetActions(availableActions);

			// check what actions can run using their checkProceduralPrecondition
			GoapActions usableActions = new GoapActions();

			foreach (GoapAction action in availableActions)
			{
				if (action.CheckProceduralPrecondition(agent))
				{
					usableActions.Add(action);
				}
			}

			// we now have all actions that can run, stored in usableActions

			// build up the tree and record the leaf nodes that provide a solution to the goal.
			List<Node> leaves = new List<Node>();

			// build graph
			Node start = new Node(null, 0, worldState, null);
			bool success = BuildTree(start, leaves, usableActions, goal);
			

			if (!success)
			{
				Debug.Log("NO PLAN");
				return null;
			}
			else
			{
				Utilities.PrintTree(leaves, GetCheapestLeaf(leaves));

				Queue<GoapAction> actionQueue = new Queue<GoapAction>();

				GetActionList(leaves).ForEach(x => actionQueue.Enqueue(x));

				return actionQueue;
			}

		}

		private void ResetActions(GoapActions availableActions)
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
			// get its node and work back through the parents

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
		private bool BuildTree(Node parent, List<Node> leaves, GoapActions usableActions, Goals goal)
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
					StatesSet currentState = UpdatedState(parent.state, action.Effects);

					Node node = new Node(parent, parent.runningCost + action.cost, currentState, action);

					// If the current state has the conditions for achieving
					// agents goal, we have found one possible plan.
					if (InState(goal, currentState))
					{
						leaves.Add(node);
						foundOne = true;
					}
					else
					{
						// not at a solution yet, so test all the remaining actions and branch out the tree
						GoapActions subset = GetActionsSubset(usableActions, action);
						Utilities.PrintCollection("Usable Actions Subset", subset);
						bool found = BuildTree(node, leaves, subset, goal);
						
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
		 * the removeAction one. Creates a new set.
		 */
		private GoapActions GetActionsSubset(GoapActions actions, GoapAction removeAction)
		{
			GoapActions subset = new GoapActions();

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
		private bool InState(StatesSet test, StatesSet state)
		{
			bool allMatch = true;

			foreach (KeyValue t in test)
			{
				allMatch = state.Contains(t);
				
				if(!allMatch)
				{
					break;
				}
			}
			return allMatch;
		}

		/**
		 * Apply the stateChange to the currentState.
		 * If state exists with wrong value - Update value
		 * If state doesn't exist - Add state
		 */
		private StatesSet UpdatedState(StatesSet currentState, StatesSet stateChange)
		{
			Utilities.PrintCollection("Current State", currentState);

			//Make Copy of Current State -> Updated State
			StatesSet updatedState = new StatesSet();

			foreach (KeyValue state in currentState)
			{
				updatedState.Add(new KeyValue(state.Key, state.Value));
			}


			foreach (KeyValue change in stateChange)
			{

				foreach (KeyValue state in updatedState)
				{

					if (state.Key.Equals(change.Key))
					{
						Debug.Log("State Found: " + change.ToString() + " == " + state.ToString() + " ( Value different )");
						updatedState.RemoveWhere((KeyValue kvp) => { return kvp.Key.Equals(change.Key); });
						
						break;
					}
				}

				updatedState.Add(new KeyValue(change.Key, change.Value));

			}

			Utilities.PrintCollection("Updated State", updatedState);
			return updatedState;
		}

		

		
		


	}
}




