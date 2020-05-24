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
	private GameObject locationPointer;

	private Vector3 previousDestination = Vector3.positiveInfinity;
	
	private bool isLookAtActive = false;

	[SerializeField] private float rotationSpeed = 5f;

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

	public IEnumerator LookAtTarget()
	{
		isLookAtActive = true;

		while (isLookAtActive && navigationTarget != null)
		{
			while (!CheckAngle(navigationTarget))
			{
				Vector3 dir = navigationTarget.transform.position - agent.transform.position;
				dir.y = 0;//This allows the object to only rotate on its y axis

				if (!dir.Equals(Vector3.zero))
				{
					Quaternion rot = Quaternion.LookRotation(dir);
					agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, rot, rotationSpeed * Time.deltaTime);
				}

				yield return null;
			}
			yield return null;
		}
	}

	public void OnDestroy()
	{
		UnityEngine.Object.Destroy(navigationTarget);
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
		if (navigationTarget == null) return;

		Vector3 destination = navigationTarget.transform.position;

		// Target is moving - Recalculate path
		if (Vector3.Distance(destination, previousDestination) > 2f)
		{
			previousDestination = destination;
			SetNavMeshAgent(destination, action.maxRequiredRange);
			navMeshAgent.SetPath(path);
		}
		
		if (path != null && path.status == NavMeshPathStatus.PathComplete)
		{
			if (navMeshAgent.remainingDistance <= action.maxRequiredRange)
			{
				action.IsInRange = true;
			}
		}
		else
		{
			InvalidateTarget();
			action.InvalidTargetLocation();
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
		if(navigationTarget != null)
		{
			if (navigationTarget.Equals(locationPointer))
			{
				navigationTarget.SetActive(false);
			}
			navigationTarget = null;
		}
		path = null;
		previousDestination = Vector3.positiveInfinity;
		isLookAtActive = false;
	}

	public bool IsTargetValid()
	{
		return navigationTarget != null 
			&& navigationTarget.activeSelf
			&& path != null
			&& path.status == NavMeshPathStatus.PathComplete;
	}

	public GameObject GetNavigationTarget()
	{
		return navigationTarget;
	}

	public void SetTarget()
	{
		if (!IsTargetValid())
		{
			SetTargetLocation(World.Instance.GetRandomLocation());
		}
	}

	public void SetTarget(GameObject actionTarget)
	{
		if (actionTarget != null)
		{
			navigationTarget = actionTarget;
		}
	}

	public void SetTarget(Vector3 targetLocation)
	{
		if (!IsTargetValid())
		{
			SetTargetLocation(targetLocation);
		}
	}

	// Runaway Navigation Target
	public void SetRunFromTarget(Vector3 runDirection)
	{
		if(!IsTargetValid())
		{
			CreateRunawayLocation(runDirection);
		}
	}
	
	private GameObject CreateRunawayLocation(Vector3 direction)
	{
		Vector3 runTo = direction;

		runTo.x *= UnityEngine.Random.Range(-30, 30);
		runTo.z *= UnityEngine.Random.Range(10, 30);

		return SetTargetLocation(runTo);
	}

	private GameObject SetTargetLocation(Vector3 targetPosition)
	{
		navigationTarget = locationPointer;
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

		locationPointer = navigationTarget;
	}
}