using GOAP;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AINavigation : NavigationController
{
	[SerializeField] private float lookAtSpeed = 5f;
	[SerializeField] private float lookAtAngle = 5f;
	[SerializeField] private float followDifference = 0.5f;

	// Target can reference target location
	// set by Action or Location pointer.
	public GameObject Target { get; private set; }

	private GameObject agent;

	private NavMeshAgent navMeshAgent;
	private NavMeshPath path;

	private GameObject locationPointer;

	private Vector3 previousPosition = Vector3.positiveInfinity;
	private Vector3 previousDestination = Vector3.positiveInfinity;

	private Coroutine LookAtCoroutine;
	private Coroutine FollowCoroutine;

	private bool isRotating = false;

	public void Initialize(GameObject agent)
	{
		this.agent = agent;
		navMeshAgent = agent.GetComponent<NavMeshAgent>();
		InstantiateLocationPointer();
	}

	protected override void IncreaseSpeed()
	{
		base.IncreaseSpeed();
		navMeshAgent.speed = currentSpeed;
		navMeshAgent.avoidancePriority -= 10;
		
	}
	protected override void DecreaseSpeed()
	{
		base.DecreaseSpeed();
		navMeshAgent.speed = currentSpeed;
		navMeshAgent.avoidancePriority += 10;
	}

	private void Update()
	{
		OnMovement(navMeshAgent.velocity.magnitude > 0f || isRotating);
	}


	public bool IsLookingAtTarget()
	{
		if (Target != null)
		{
			return Utilities.GetAngle(agent, Target) < lookAtAngle;
		}
		return true;
	}
	public void LookAtTarget()
	{
		StopLookAt();
		LookAtCoroutine = StartCoroutine(LookAt());
	}
	public void StopLookAt()
	{
		if (LookAtCoroutine != null)
		{
			StopCoroutine(LookAtCoroutine);
			LookAtCoroutine = null;
		}

	}
	private IEnumerator LookAt()
	{
		while (Target != null)
		{
			while (!IsLookingAtTarget())
			{
				Vector3 dir = Target.transform.position - agent.transform.position;
				dir.y = 0; //This allows the object to only rotate on its y axis

				if (!dir.Equals(Vector3.zero))
				{
					isRotating = true;
					Quaternion rot = Quaternion.LookRotation(dir);
					agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, rot, lookAtSpeed * Time.deltaTime);
				}

				yield return null;
			}
			isRotating = false;
			yield return null;
		}

	}

	

	public void FollowTarget(float maxRange)
	{
		StopFollow();
		FollowCoroutine = StartCoroutine(Follow(maxRange));
	}
	public void StopFollow()
	{
		if (FollowCoroutine != null)
		{
			StopCoroutine(FollowCoroutine);
			FollowCoroutine = null;
		}
	}
	private IEnumerator Follow(float maxRange)
	{
		while (Target != null)
		{
			Vector3 position = Target.transform.position;

			if (Vector3.Distance(position, previousPosition) > followDifference)
			{
				previousPosition = position;
				SetNavMeshAgent(position, maxRange - 2);
				navMeshAgent.SetPath(path);
			}

			yield return null;
		}
	}

	public void Move(GoapAction action)
	{
		if (Target == null) return;

		Vector3 destination = Target.transform.position;

		// Target is moving - Recalculate path
		if (Vector3.Distance(destination, previousDestination) > 2f)
		{
			previousDestination = destination;
			SetNavMeshAgent(destination, action.maxRequiredRange);
			navMeshAgent.SetPath(path);
		}
		
		if (IsPathValid())
		{
			if (navMeshAgent.remainingDistance <= action.maxRequiredRange)
			{
				action.IsInRange = true;
			}
		}
		else
		{
			InvalidateTarget();
			action.ResetTarget();
		}
	}


	private void SetNavMeshAgent(Vector3 destination, float stoppingDistance)
	{
		path = new NavMeshPath();
		navMeshAgent.CalculatePath(destination, path);
		navMeshAgent.stoppingDistance = stoppingDistance;
	}

	public void InvalidateTarget()
	{
		if(Target != null)
		{
			if (!Target.Equals(locationPointer))
			{
				Target = locationPointer;
			}

			Target.SetActive(false);
		}

		path = null;
		previousDestination = Vector3.positiveInfinity;
		previousPosition = Vector3.positiveInfinity;
	}

	private bool IsTargetValid()
	{
		return Target != null && Target.activeSelf;
	}
	private bool IsPathValid()
	{
		return path != null && path.status == NavMeshPathStatus.PathComplete;
	}

	public void SetTarget(GameObject actionTarget)
	{
		if (actionTarget != null)
		{
			Target = actionTarget;
		}
	}

	public void SetSearchTarget()
	{
		if (!IsTargetValid() || !IsPathValid())
		{
			// Find forward direction of agent
			Vector3 direction = agent.transform.forward.normalized;
			Vector3 location = World.Instance.CreateLocation(agent, direction, 10, 30);
			// Set run location
			SetTargetLocation(location);
		}
	}

	public void SetRunAwayTarget(Vector3 runFromTarget)
	{
		if(!IsTargetValid() || !IsPathValid())
		{
			// Find direction from where agent is attacked
			Vector3 direction = (runFromTarget - agent.transform.position).normalized;
			Vector3 location = World.Instance.CreateRunAwayLocation(agent, direction, 15, 30);

			// Set run location
			SetTargetLocation(location);
		}
	}
	

	private void SetTargetLocation(Vector3 targetPosition)
	{
		Target = locationPointer;
		Target.transform.position = World.Instance.ClampToWorld(targetPosition);
		Target.transform.rotation = Quaternion.identity;
		Target.SetActive(true);
	}

	private void InstantiateLocationPointer()
	{
		Target = new GameObject("TargetLocation ( " + agent.name + " )");
		Target.transform.parent = World.Instance.worldLocations;
		Target.SetActive(false);

		locationPointer = Target;
	}

	private void OnDestroy()
	{
		Destroy(locationPointer);
	}
}