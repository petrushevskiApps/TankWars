using GOAP;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AINavigation : NavigationController
{

	// Target can reference target location
	// set by Action or Location pointer.
	public GameObject Target { get; private set; }

	[SerializeField] private float lookAtSpeed = 5f;
	[SerializeField] private float lookAtAngle = 5f;
	[SerializeField] private float followDifference = 0.5f;

	private GameObject agent;
	private AIAgent aiAgent;

	private NavMeshAgent navMeshAgent;
	private NavMeshPath path;

	private GameObject locationPointer;

	private Vector3 previousPosition = Vector3.positiveInfinity;
	private Vector3 previousDestination = Vector3.positiveInfinity;

	private Coroutine LookAtCoroutine;
	private Coroutine FollowCoroutine;

	private bool isRotating = false;
	private bool isBoostOn = false;

	public void Initialize(GameObject agent, int agentID)
	{
		this.agent = agent;
		aiAgent = agent.GetComponent<AIAgent>();
		navMeshAgent = agent.GetComponent<NavMeshAgent>();
		//navMeshAgent.avoidancePriority += agentID;
		InstantiateLocationPointer();
	}

	public override void BoostSpeed()
	{
		base.BoostSpeed();
		navMeshAgent.speed = currentSpeed;
		navMeshAgent.avoidancePriority -= 10;
		isBoostOn = true;
	}
	public override void ResetSpeed()
	{
		base.ResetSpeed();
		navMeshAgent.speed = currentSpeed;
		navMeshAgent.avoidancePriority += 10;
		isBoostOn = false;
	}

	private void Update()
	{
		OnMovement(navMeshAgent.velocity.magnitude > 0f || isRotating);

		if (isBoostOn)
		{
			if (aiAgent.Inventory.SpeedBoost.Amount > 0)
			{
				aiAgent.BoostOn();
			}
			else
			{
				aiAgent.BoostOff();
			}
		}
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

	public bool IsFollowing { get; private set; } = false;

	public void FollowTarget(float maxRange)
	{
		StopFollow();
		FollowCoroutine = StartCoroutine(Follow(maxRange));
	}
	public void StopFollow()
	{
		if (FollowCoroutine != null)
		{
			IsFollowing = false;
			StopCoroutine(FollowCoroutine);
			FollowCoroutine = null;
		}
	}
	private IEnumerator Follow(float maxRange)
	{
		while (Target != null)
		{
			IsFollowing = true;
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
			action.InvalidTargetLocation();
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

			CreateLocation(direction);
		}
	}

	public void SetRunAwayTarget(Vector3 runFromTarget)
	{
		if(!IsTargetValid() || !IsPathValid())
		{
			// Find direction from where agent is attacked
			Vector3 direction = (runFromTarget - agent.transform.position).normalized;

			CreateLocation(direction);
		}
	}
	private void CreateLocation(Vector3 direction)
	{
		// Add range to direction
		direction *= Random.Range(10, 50);

		// Find up direction of agent
		Vector3 upDirection = agent.transform.up.normalized;

		// Get side direction
		Vector3 sideDirection = Vector3.Cross(direction, upDirection);

		sideDirection *= Random.Range(0, 50) * (Random.Range(0f, 1f) <= 0.5 ? -1 : 1);

		// Add side direction to forward
		direction += sideDirection;

		// Find location in opposite direction of the attack
		Vector3 runToLocation = direction + agent.transform.position;

		// Set run location
		SetTargetLocation(World.Instance.ClampToWorld(runToLocation));
	}

	private void SetTargetLocation(Vector3 targetPosition)
	{
		Target = locationPointer;
		Target.transform.position = targetPosition;
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