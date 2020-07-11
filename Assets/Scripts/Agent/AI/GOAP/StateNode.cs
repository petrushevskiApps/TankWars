using System;
using System.Collections.Generic;

namespace GOAP
{

	/**
	* Used for building up the graph and holding the running costs of actions.
	*/
	public class StateNode : IEquatable<StateNode>, IComparable<StateNode>
    {
		public StateNode parent;
		
		public float StateCost => heuristic + RunningCost;

		private float heuristic = 0;
		public float RunningCost { get; private set; }

		private Dictionary<string, bool> goalState;
		public Dictionary<string, bool> State { get; private set; }
		public GoapAction action; 

		public StateNode(StateNode parent, float runningCost, Dictionary<string, bool> state, GoapAction action, Dictionary<string, bool> goalState)
		{
			this.parent = parent;
			this.RunningCost = runningCost;
			this.State = state;
			this.action = action;
			this.goalState = goalState;
			SetHeuristic();
		}
		private void SetHeuristic()
		{
			foreach(KeyValuePair<string, bool> gsPair in goalState)
			{
				if(State.ContainsKey(gsPair.Key))
				{
					// Increase heuristic cost if condition
					// is found in state but with different value
					if (gsPair.Value != State[gsPair.Key])
					{
						heuristic++;
					}
				}
				else
				{
					// Increase heuristic cost if condition
					// is not found in state
					heuristic++;
				}
			}
		}
        public int CompareTo(StateNode other)
        {
            if (StateCost > other.StateCost)
            {
                return 1;
            }
            else if (StateCost < other.StateCost)
            {
                return -1;
            }
            else
            {
				if (heuristic < other.heuristic)
				{
					return -1;
				}
				else if(heuristic > other.heuristic)
				{
					return 1;
				}
				else
				{
					return 0;
				}
            }

        }
        public bool Equals(StateNode other)
        {
			if(State == null || other.State == null)
			{
				return false;
			}
			else
			{
				if (State.Count != other.State.Count)
				{
					return false;
				}
				else
				{
					foreach (KeyValuePair<string, bool> kvp in State)
					{
						if (other.State.ContainsKey(kvp.Key))
						{
							bool otherValue = other.State[kvp.Key];

							if (otherValue != kvp.Value)
							{
								return false;
							}
						}
						else return false;
					}
					return true;
				}
			}
			
        }
        public override string ToString()
		{
			if (parent != null)
			{
				return "(" + StateCost.ToString() + " = " + RunningCost + " + " + heuristic + ")"
					+ action.GetType().ToString() + ": \n" + Utilities.PrintDictionary(State) + "\n";
			}
			else
			{
				return "(" + StateCost.ToString() + " = " + RunningCost + " + " + heuristic + ")"
					+ " root " + ": \n" + Utilities.PrintDictionary(State) + "\n";
			}
		}
	}
}
