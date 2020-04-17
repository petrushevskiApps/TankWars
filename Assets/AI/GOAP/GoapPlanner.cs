using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GOAP
{
	using WorldState = HashSet<KeyValuePair<string, object>>;
	using Goals = HashSet<KeyValuePair<string, object>>;
	using GoapActions = HashSet<GoapAction>;

	/**
	 * Plans what actions can be completed in order to fulfill a goal state.
	 */
	public class GoapPlanner
	{

		
		private void ResetActions(GoapActions availableActions)
		{
			foreach (GoapAction action in availableActions )
			{
				action.Reset();
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
				else
				{
					if (leaf.runningCost < result.runningCost)
					{
						result = leaf;
					}
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

			PrintCollection("Actions", result);

			return result;
		}
		
		/**
		 * Plan what sequence of actions can fulfill the goal.
		 * Returns null if a plan could not be found, or a list of the actions
		 * that must be performed, in order, to fulfill the goal.
		 */
		public Queue<GoapAction> Plan(GameObject agent, GoapActions availableActions, WorldState worldState, Goals goal)
		{
			// reset the actions so we can start fresh with them
			ResetActions(availableActions);

			// check what actions can run using their checkProceduralPrecondition
			GoapActions usableActions = new GoapActions();

			foreach (GoapAction action in availableActions)
			{
				if (action.checkProceduralPrecondition(agent))
				{
					usableActions.Add(action);
					//Debug.Log("Action: " + a.name);
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
				Queue<GoapAction> queue = new Queue<GoapAction>();

				GetActionList(leaves).ForEach(x => queue.Enqueue(x));

				return queue;
			}

		}

		private void PrintCollection<T>(string setName, ICollection<T> set)
		{
			StringBuilder sb = new StringBuilder();
			foreach (T element in set)
			{
				sb.Append(element);
			}
			Debug.Log(setName + " Count: " + set.Count + " List:" + sb.ToString());
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

			PrintCollection("Usable Actions", usableActions);

			// go through each action available at this node and see if we can use it here
			foreach (GoapAction action in usableActions)
			{
				
				// if the parent state has the conditions for this action's preconditions, we can use it here
				if (InState(action.Preconditions, parent.state))
				{

					// apply the action's effects to the parent state
					WorldState currentState = PopulateState(parent.state, action.Effects);

					Node node = new Node(parent, parent.runningCost + action.cost, currentState, action);

					if (InState(goal, currentState))
					{
						// we found a solution!
						leaves.Add(node);
						foundOne = true;
					}
					else
					{
						// not at a solution yet, so test all the remaining actions and branch out the tree
						GoapActions subset = ActionSubset(usableActions, action);
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
		 * Create a subset of the actions excluding the removeMe one. Creates a new set.
		 */
		private GoapActions ActionSubset(GoapActions actions, GoapAction removeMe)
		{
			GoapActions subset = new GoapActions();
			
			foreach (GoapAction a in actions)
			{
				if (!a.Equals(removeMe))
				{
					subset.Add(a);
				}
			}

			return subset;
		}

		/**
		 * Check that all items in 'test' are in 'state'. If just one does not match or is not there
		 * then this returns false.
		 */
		private bool InState(WorldState test, WorldState state)
		{
			bool allMatch = true;
			
			foreach (KeyValuePair<string, object> t in test)
			{
				bool match = false;
				foreach (KeyValuePair<string, object> s in state)
				{
					if (s.Equals(t))
					{
						match = true;
						break;
					}
				}
				if (!match)
				{
					allMatch = false;
				}
			}
			return allMatch;
		}

		/**
		 * Apply the stateChange to the currentState
		 */
		private HashSet<KeyValuePair<string, object>> PopulateState(HashSet<KeyValuePair<string, object>> currentState, HashSet<KeyValuePair<string, object>> stateChange)
		{
			HashSet<KeyValuePair<string, object>> state = new HashSet<KeyValuePair<string, object>>();
			// copy the KVPs over as new objects
			foreach (KeyValuePair<string, object> s in currentState)
			{
				state.Add(new KeyValuePair<string, object>(s.Key, s.Value));
			}

			foreach (KeyValuePair<string, object> change in stateChange)
			{
				// if the key exists in the current state, update the Value
				bool exists = false;

				foreach (KeyValuePair<string, object> s in state)
				{
					if (s.Equals(change))
					{
						exists = true;
						break;
					}
				}

				if (exists)
				{
					state.RemoveWhere((KeyValuePair<string, object> kvp) => { return kvp.Key.Equals(change.Key); });
					KeyValuePair<string, object> updated = new KeyValuePair<string, object>(change.Key, change.Value);
					state.Add(updated);
				}
				// if it does not exist in the current state, add it
				else
				{
					state.Add(new KeyValuePair<string, object>(change.Key, change.Value));
				}
			}
			return state;
		}

		/**
		 * Used for building up the graph and holding the running costs of actions.
		 */
		private class Node
		{
			public Node parent;
			public float runningCost;
			public HashSet<KeyValuePair<string, object>> state;
			public GoapAction action;

			public Node(Node parent, float runningCost, HashSet<KeyValuePair<string, object>> state, GoapAction action)
			{
				this.parent = parent;
				this.runningCost = runningCost;
				this.state = state;
				this.action = action;
			}
		}

	}
}




