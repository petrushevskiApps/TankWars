using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GOAP;

public class Navigation
{
	private GameObject agent;
	private GameObject actionTarget;
	private GameObject targetLocation;

	public Navigation(GameObject agent)
	{
		targetLocation = new GameObject("targetLocation");
		targetLocation.SetActive(false);

		this.agent = agent;
	}

	// Set specific target object
	public void SetTarget(GameObject target)
	{
		this.actionTarget = target;
	}

	// Create new target object
	public GameObject SetTarget()
	{
		targetLocation.transform.position = CornerCalculator.Instance.GetRandomInWorldCoordinates();
		targetLocation.transform.rotation = Quaternion.identity;
		targetLocation.SetActive(true);

		return targetLocation;
	}

	public GameObject SetRunawayTarget(GameObject otherTarget)
	{
		
		targetLocation.transform.position = (agent.transform.InverseTransformDirection(Vector3.forward) * (-1)) * (5);
		targetLocation.transform.rotation = Quaternion.identity;
		targetLocation.SetActive(true);

		return targetLocation;
	}
	public GameObject GetActionTarget(GameObject otherTarget = null)
	{
		if (actionTarget == null)
		{
			if(otherTarget != null)
			{
				actionTarget = SetRunawayTarget(otherTarget);
			}
			else
			{
				actionTarget = SetTarget();
			}
			return actionTarget;
		}
		else return actionTarget;
	}

	public void TargetReached(GameObject target)
	{
		if(target.Equals(this.actionTarget))
		{
			actionTarget = null;
			targetLocation.SetActive(false);
		}
	}

	public void InvalidateTarget()
	{
		actionTarget = null;
	}
}