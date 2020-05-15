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
		public static float GetAngle(Transform caller, Vector3 target)
		{
			return Vector3.Angle(caller.forward, GetDirection(caller, target));
		}
		public static Vector3 GetDirection(Transform caller, Vector3 target)
		{
			Vector3 heading = (target - caller.position);
			float distance = heading.magnitude;
			return heading / distance;
		}

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

		public static void PrintGOAPPlan(string agentName, List<Node> leaves)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append($"{agentName} | GOAP Tree: \n");

			foreach (Node leaf in leaves)
			{
				sb.Append(leaf.ToString());
				sb.Append("->");
			}
			sb.Append("Goal");
			Debug.Log(sb.ToString());
		}

		public static void PrintCollection<T>(string setName, ICollection<T> set)
		{
			StringBuilder sb = new StringBuilder();
			foreach (T element in set)
			{
				sb.Append(element);
			}
			//Debug.Log(setName + " Count: " + set.Count + " List:" + sb.ToString());
		}
		public static string GetCollectionString<T>(ICollection<T> set)
		{
			set.Reverse<T>();

			StringBuilder sb = new StringBuilder();
			
			foreach (T element in set)
			{
				sb.Append(element.GetType().ToString());
				sb.Append("->");
			}
			return sb.ToString();
		}
		
	}
}
