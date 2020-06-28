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
		public static float GetAngle(GameObject caller, GameObject target)
		{
			return Vector3.Angle(caller.transform.forward, GetDirection(caller, target));
		}

		public static Vector3 GetDirection(Transform caller, Vector3 target)
		{
			Vector3 heading = (target - caller.position);
			float distance = heading.magnitude;
			return heading / distance;
		}

		public static Vector3 GetDirection(GameObject caller, GameObject target)
		{
			if(target != null)
			{
				Vector3 heading = (target.transform.position - caller.transform.position);
				float distance = heading.magnitude;
				return heading / distance;
			}
			return Vector3.forward;
		}

		public static float GetDistance(GameObject start, GameObject end)
		{
			if (start != null && end != null)
			{
				return Vector3.Distance(start.transform.position, end.transform.position);
			}
			else return Mathf.Infinity;
		}

		public static int CompareDistances(Vector3 position, Vector3 targetPostion1, Vector3 targetPostion2)
		{
			float d1 = Vector3.Distance(position, targetPostion1);
			float d2 = Vector3.Distance(position, targetPostion2);

			if (d1 < d2) return -1;
			else if (d1 > d2) return 1;
			else return 0;
		}
		

		public static void PrintCollection<T>(string setName, ICollection<T> set)
		{
			StringBuilder sb = new StringBuilder();
			foreach (T element in set)
			{
				sb.Append(element);
			}
		}

		public static string PrintDictionary(Dictionary<string, bool> dictionary)
		{
			StringBuilder sb = new StringBuilder();
			foreach(KeyValuePair<string, bool> entry in dictionary)
			{
				sb.Append(entry.Key + ": " + entry.Value);
				sb.AppendLine();
			}
			return sb.ToString();
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

		// Time to reach the target position counted in seconds
		public static float TimeToReach(Vector3 start, GameObject target, float speed)
		{
			if (target != null)
			{
				Vector3 destination = target.transform.position;
				float distance = Vector3.Distance(start, destination);
				return distance / speed;
			}
			else return 20;
		}
	}
}
