using System;
using System.Collections.Generic;

namespace GOAP
{

	/**
	* Used for building up the graph and holding the running costs of actions.
	*/
	public class Node : IEquatable<Node>, IComparable<Node>
    {
		public Node parent;
		public float StateCost => GetHeuristic() + runningCost;

		public float runningCost;

		private Dictionary<string, bool> goalState;
		public Dictionary<string, bool> state;
		public GoapAction action; 

		public Node(Node parent, float runningCost, Dictionary<string, bool> state, GoapAction action, Dictionary<string, bool> goalState)
		{
			this.parent = parent;
			this.runningCost = runningCost;
			this.state = state;
			this.action = action;
			this.goalState = goalState;
		}
		private float GetHeuristic()
		{
			float differences = 0;

			foreach(KeyValuePair<string, bool> gState in goalState)
			{
				foreach (KeyValuePair<string, bool> cState in state)
				{
					if(gState.Key.Equals(cState.Key) && gState.Value != cState.Value)
					{
						differences++;
					}
				}
			}

			return differences;
		}
        public int CompareTo(Node other)
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
                if(GetHeuristic() > other.GetHeuristic())
				{
					return 1;
				}
				else if(GetHeuristic() < other.GetHeuristic())
				{
					return -1;
				}
				else
				{
					return 0;
				}
            }

        }

        public bool Equals(Node other)
        {
			if((state != null && other.state == null) || (state == null && other.state != null))
			{
				return false;
			}
			else if(other.state.Count != state.Count)
			{
				return false;
			}
			else
			{
				foreach(KeyValuePair<string, bool> kvp in state)
				{
					if (other.state.ContainsKey(kvp.Key))
					{
						bool otherValue = other.state[kvp.Key];
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

        public override string ToString()
		{
			if (parent != null)
			{
				return "(" + StateCost.ToString() + ")" + action.actionName.ToString();
			}
			else
			{
				return "root";
			}

		}
	}
}
