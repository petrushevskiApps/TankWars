using GOAP;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]

public class NavigationSystem
{
	private GameObject agent;
	private NavMeshAgent navMeshAgent;
	private NavMeshPath path;

	private GameObject navigationTarget;
	private GameObject internalCachedTarget;

	private Vector3 previousDestination = Vector3.zero;
	
	private bool abortMovement = false;
	private bool isLookAtActive = false;
	

	public NavigationSystem(GameObject agent)
	{
		this.agent = agent;
		navMeshAgent = agent.GetComponent<NavMeshAgent>();

		InstantiateLocationPointer();
	}

	public bool IsLookingAtTarget()
	{
		return CheckAngle(navigationTarget);
	}

	public IEnumerator LookAtTarget(GameObject lookAtTarget)
	{
		isLookAtActive = true;

		while (isLookAtActive && lookAtTarget != null)
		{
			while (!CheckAngle(lookAtTarget))
			{
				Vector3 dir = lookAtTarget.transform.position - agent.transform.position;
				dir.y = 0;//This allows the object to only rotate on its y axis

				if (!dir.Equals(Vector3.zero))
				{
					Quaternion rot = Quaternion.LookRotation(dir);
					agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, rot, 10f * Time.deltaTime);
				}

				yield return null;
			}
			yield return null;
		}
	}

	private bool CheckAngle(GameObject target)
	{
		if(target != null)
		{
			float angle = Utilities.GetAngle(agent, target);
			return angle < 5f;
		}
		return true;
	}

	public void Move(GoapAction action)
	{
		Vector3 destination = navigationTarget.transform.position;

		// Target is moving - Recalculate path
		if (Vector3.Distance(destination, previousDestination) > 2f)
		{
			previousDestination = destination;
			SetNavMeshAgent(destination, action.maxRequiredRange);
		}
		
		if (path != null && path.status == NavMeshPathStatus.PathComplete)
		{
			navMeshAgent.SetPath(path);
			bool inRange = navMeshAgent.remainingDistance <= action.maxRequiredRange;

			if (inRange)
			{
				action.IsInRange = true;
			}
		}
		else
		{
			InvalidateTarget();
			SetTarget();
		}
	}
	private void SetNavMeshAgent(Vector3 destination, float stoppingDistance)
	{
		path = new NavMeshPath();
		navMeshAgent.CalculatePath(destination, path);
		navMeshAgent.stoppingDistance = stoppingDistance;
		navMeshAgent.updateRotation = true;
	}

	public void InvalidateTarget()
	{
		isLookAtActive = false;
		path = null;

		navigationTarget = internalCachedTarget;
		navigationTarget.SetActive(false);
	}

	public bool IsTargetSet()
	{
		return navigationTarget != null && navigationTarget.activeSelf;
	}

	public GameObject GetNavigationTarget()
	{
		return navigationTarget;
	}

	public void SetTarget(GameObject actionTarget)
	{
		if (actionTarget != null)
		{
			navigationTarget = actionTarget;
		}
	}

	public void SetTarget()
	{
		if (!IsTargetSet())
		{
			SetTargetLocation(World.Instance.GetRandomLocation());
		}
	}
	public void SetTarget(Vector3 targetLocation)
	{
		if (!IsTargetSet())
		{
			SetTargetLocation(targetLocation);
		}
	}

	// Runaway Navigation Target
	public void SetRunFromTarget(GameObject runFrom)
	{
		if(!IsTargetSet() && runFrom != null)
		{
			CreateRunawayLocation(runFrom);
		}
	}
	
	private GameObject CreateRunawayLocation(GameObject otherTarget)
	{
		Vector3 runTo = agent.transform.forward * (-1);

		runTo.x *= UnityEngine.Random.Range(-30, 30);
		runTo.z *= UnityEngine.Random.Range(10, 30);

		return SetTargetLocation(runTo);
	}

	private GameObject SetTargetLocation(Vector3 targetPosition)
	{
		if(navigationTarget == null)
		{
			navigationTarget = internalCachedTarget;
		}

		navigationTarget.transform.position = targetPosition;
		navigationTarget.transform.rotation = Quaternion.identity;
		navigationTarget.SetActive(true);

		return navigationTarget;
	}

	private void InstantiateLocationPointer()
	{
		navigationTarget = new GameObject("TargetLocation ( " + agent.name + " )");
		navigationTarget.transform.parent = World.Instance.worldLocations;
		navigationTarget.SetActive(false);

		internalCachedTarget = navigationTarget;
	}
}