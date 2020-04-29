using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GOAP;
using UnityEngine.AI;

public class Navigation
{
	private GameObject agent;
	private NavMeshAgent navMeshAgent;
	private NavMeshPath path;

	private GameObject target;
	private GameObject targetLocation;

	private Vector3 previousDestination = Vector3.zero;
	private bool abortMovement = false;

	public Navigation(GameObject agent)
	{
		targetLocation = new GameObject("targetLocation");
		targetLocation.SetActive(false);

		this.agent = agent;
		navMeshAgent = agent.GetComponent<NavMeshAgent>();
	}

	public void MoveAgent(GoapAction action)
	{
		Vector3 destination = target.transform.position;
		
		if (Vector3.Distance(destination, previousDestination) > 0.05f)
		{
			// Target is moving - Recalculate path
			previousDestination = destination;

			path = new NavMeshPath();
			navMeshAgent.CalculatePath(destination, path);
			navMeshAgent.stoppingDistance = action.maxRequiredRange;
			navMeshAgent.updateRotation = true;
		}
		if (path.status == NavMeshPathStatus.PathComplete)
		{
			navMeshAgent.SetPath(path);
		}
		else
		{
			InvalidateTarget();
		}
	}

	public bool IsAgentOnTarget(GoapAction action)
	{
		if (path != null)
		{
			bool isOnTarget = navMeshAgent.remainingDistance <= action.maxRequiredRange;
			if (isOnTarget || abortMovement)
			{
				action.SetInRange(true);
				InvalidateTarget();
				return true;
			}
		}

		return false;
	}

	public void AbortMoving()
	{
		abortMovement = true;
	}
	public bool IsTargetSet()
	{
		return target != null;
	}

	// Create new target object
	public GameObject CreateTarget()
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

	public GameObject SetTarget(GameObject actionTarget = null)
	{
		if(actionTarget != null)
		{
			target = actionTarget;
		}
		else
		{
			if(target == null)
			{
				target = CreateTarget();
			}
		}
		
		return target;
	}
	public GameObject GetTarget()
	{
		return target;
	}
	public void InvalidateTarget()
	{
		abortMovement = false;
		path = null;
		target = null;
		targetLocation.SetActive(false);
	}
}