using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GOAP
{
    public class Utilities
    {
		public static float GetAngle(GameObject caller, GameObject target)
		{
			return Vector3.Angle(caller.transform.forward, GetDirection(caller, target));
		}

		public static Vector3 GetDirection(GameObject caller, GameObject target)
		{
			Vector3 heading = (target.transform.position - caller.transform.position);
			float distance = heading.magnitude;
			return heading / distance;
		}

		public static void PrintTree(List<Node> leaves, Node cheapestLeaf)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("GOAP Tree: \n");

			foreach (Node leaf in leaves)
			{
				Node node = leaf;

				if (leaf.Equals(cheapestLeaf))
				{
					sb.Append("<b><color=green> ");
				}
				while (node != null)
				{
					sb.Append(node.ToString());
					node = node.parent;
				}
				if (leaf.Equals(cheapestLeaf))
				{
					sb.Append(" </color></b>");
				}

				sb.Append("\n");
			}

			Debug.Log(sb.ToString());
		}

		public static void PrintCollection<T>(string setName, ICollection<T> set)
		{
			StringBuilder sb = new StringBuilder();
			foreach (T element in set)
			{
				sb.Append(element);
			}
			Debug.Log(setName + " Count: " + set.Count + " List:" + sb.ToString());
		}


	}
}
