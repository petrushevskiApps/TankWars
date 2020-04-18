using System.Collections.Generic;

namespace GOAP
{
	using StatesSet = HashSet<KeyValuePair<string, object>>;

	/**
	* Used for building up the graph and holding the running costs of actions.
	*/
	public class Node
    {
		public Node parent;
		public float runningCost;
		public StatesSet state;
		public GoapAction action;

		public Node(Node parent, float runningCost, StatesSet state, GoapAction action)
		{
			this.parent = parent;
			this.runningCost = runningCost;
			this.state = state;
			this.action = action;
		}

		public override string ToString()
		{
			if (parent != null)
			{
				return "(" + runningCost.ToString() + ")" + action.name.ToString() + "->";
			}
			else
			{
				return "root";
			}

		}
	}
}
