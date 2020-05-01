using GOAP;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]

public class NavigationSystem
{
	private GameObject agent;
	private NavMeshAgent navMeshAgent;
	private NavMeshPath path;

	[SerializeField] GameObject target;
	private GameObject targetLocation;

	private Vector3 previousDestination = Vector3.zero;
	
	private bool abortMovement = false;
	private bool isLookAtActive = false;

	public NavigationSystem(GameObject agent)
	{
		targetLocation = new GameObject("targetLocation");
		targetLocation.SetActive(false);

		this.agent = agent;
		navMeshAgent = agent.GetComponent<NavMeshAgent>();
	}

	public bool IsLookingAtTarget()
	{
		return CheckAngle();
	}

	public IEnumerator LookAtTarget()
	{
		isLookAtActive = true;

		while (isLookAtActive && target != null)
		{
			while (!CheckAngle())
			{
				Vector3 dir = target.transform.position - agent.transform.position;
				dir.y = 0;//This allows the object to only rotate on its y axis

				if (!dir.Equals(Vector3.zero))
				{
					Quaternion rot = Quaternion.LookRotation(dir);
					agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, rot, 5f * Time.deltaTime);
				}

				yield return null;
			}

			yield return null;
		}
	}

	private bool CheckAngle()
	{
		float angle = Utilities.GetAngle(agent, target);
		return angle < 5;
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
		if (path != null && path.status == NavMeshPathStatus.PathComplete)
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

	public GameObject GetTarget()
	{
		return target;
	}

	public GameObject SetRunFromTarget(GameObject runFromTarget)
	{
		if(target == null && runFromTarget != null)
		{
			target = CreateRunawayTarget(runFromTarget);
		}
		
		return target;
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
	

	// Create new target object
	private GameObject CreateTarget()
	{
		targetLocation.transform.position = CornerCalculator.Instance.GetRandomInWorldCoordinates();
		targetLocation.transform.rotation = Quaternion.identity;
		targetLocation.SetActive(true);

		return targetLocation;
	}

	private GameObject CreateRunawayTarget(GameObject otherTarget)
	{
		Vector3 runTo = otherTarget.transform.forward;
		runTo.x += Random.Range(-20, 20);
		runTo.z += Random.Range(20, 30);

		targetLocation.transform.position = runTo;
		targetLocation.transform.rotation = Quaternion.identity;
		targetLocation.SetActive(true);

		return targetLocation;
	}

	public void InvalidateTarget()
	{
		isLookAtActive = false;
		abortMovement = false;
		path = null;
		target = null;
		targetLocation.SetActive(false);
	}
}