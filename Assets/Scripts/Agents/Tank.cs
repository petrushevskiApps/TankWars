using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.Events;
using System;
using GOAP;

public class Tank : MonoBehaviour, IGoap
{
	NavMeshAgent agent;
	Vector3 previousDestination;

	public Memory agentMemory = new Memory();

	[SerializeField] private VisionController visionSensors;


	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();

		visionSensors.EnemyDetectedEvent.AddListener(OnEnemyDetected);
		visionSensors.EnemyLostEvent.AddListener(OnEnemyLost);
		visionSensors.HealthPackDetected.AddListener(OnHealthPackDetected);
		visionSensors.AmmoPackDetected.AddListener(OnAmmoPackDetected);
	}

	private void OnHealthPackDetected(GameObject target)
	{
		throw new NotImplementedException();
	}

	private void OnAmmoPackDetected(GameObject target)
	{
		agentMemory.AddDetectedAmmoPack(target);
	}

	private void OnDestroy()
	{
		visionSensors.EnemyDetectedEvent.RemoveListener(OnEnemyDetected);
		visionSensors.EnemyLostEvent.RemoveListener(OnEnemyLost);
		visionSensors.AmmoPackDetected.RemoveListener(OnAmmoPackDetected);
	}

	private void OnEnemyDetected(GameObject target)
	{
		agentMemory.AddDetectedEnemy(target);
	}

	private void OnEnemyLost(GameObject target)
	{
		agentMemory.RemoveDetectedEnemy(target.name);
	}


	private bool rotate = false;
	private Vector3 currentDestination = Vector3.zero;


	public bool MoveAgent(GoapAction nextAction) 
	{
		rotate = true;
		currentDestination = nextAction.target.transform.position;

		// Agent at destination point
		if(previousDestination == currentDestination)
		{
			Debug.Log("MoveAgent:: " + gameObject.name + "At Target Range");
			nextAction.SetInRange(true);
			return true;
		}

		agent.isStopped = false;
		NavMeshPath path = new NavMeshPath();
		agent.CalculatePath(currentDestination, path);
		agent.stoppingDistance = nextAction.maxRequiredRange;

		if (path.status == NavMeshPathStatus.PathComplete)
		{
			agent.SetPath(path);

			if (agent.remainingDistance <= nextAction.maxRequiredRange)
			{
				bool angleCheck = true;

				if(nextAction.requireAngle)
				{
					angleCheck = CheckAngle(nextAction.target);
				}

				if (angleCheck)
				{
					rotate = false;
					nextAction.SetInRange(true);
					previousDestination = currentDestination;
					agent.isStopped = false;
					return true;
				}
				return false;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}

	private bool CheckAngle(GameObject target)
	{
		float angle = Utilities.GetAngle(gameObject, target);
		return angle < 5;
	}

	private void Update()
	{
		if(rotate)
		{
			Vector3 dir = currentDestination - transform.position;
			dir.y = 0;//This allows the object to only rotate on its y axis
			if(!dir.Equals(Vector3.zero))
			{
				Quaternion rot = Quaternion.LookRotation(dir);
				transform.rotation = Quaternion.Lerp(transform.rotation, rot, 5f * Time.deltaTime);
			}
		}
	}

	public Dictionary<string, bool> GetWorldState()
	{
		Dictionary<string, bool> worldState = new Dictionary<string, bool>();
		Dictionary<string, Func<bool>> internalState = agentMemory.GetWorldState();

		foreach (KeyValuePair<string, Func<bool>> state in internalState)
		{
			worldState.Add(state.Key, state.Value());
		}

		return worldState;
	}

	public Dictionary<string, bool> CreateGoalState()
	{
		return agentMemory.GetGoalState();
	}

	public void PlanFailed (Dictionary<string, bool> failedGoal)
	{

	}

	public void PlanFound (Dictionary<string, bool> goal, Queue<GoapAction> actions)
	{

	}

	public void ActionsFinished ()
	{

	}

	public void PlanAborted (GoapAction aborter)
	{

	}

	
}
