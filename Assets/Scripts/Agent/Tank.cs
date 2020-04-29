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
	public NavMeshPath path;

	[SerializeField] private VisionController visionSensor; 

	private void Awake()
	{
		agentMemory.Initialize(gameObject);
		agentMemory.AddEvents(visionSensor);
		agent = GetComponent<NavMeshAgent>();
		
	}

	private void OnDestroy()
	{
		agentMemory.RemoveEvents(visionSensor);
	}
	
	public bool MoveAgent(GoapAction nextAction)
	{
		agentMemory.Navigation.MoveAgent(nextAction);
		return agentMemory.Navigation.IsAgentOnTarget(nextAction);
	}

	//public bool MoveAgent(GoapAction nextAction) 
	//{
	//	rotate = true;
	//	currentDestination = nextAction.target.transform.position;

	//	path = new NavMeshPath();
	//	agent.isStopped = false;
	//	agent.CalculatePath(currentDestination, path);
	//	agent.stoppingDistance = nextAction.maxRequiredRange;
	//	agent.updateRotation = true;

	//	if (path.status == NavMeshPathStatus.PathComplete)
	//	{
	//		agent.SetPath(path);

	//		if (agent.remainingDistance <= nextAction.maxRequiredRange)
	//		{
	//			bool requireAngle = nextAction.requireAngle;
	//			bool checkAngle = CheckAngle(nextAction.target);

	//			if (!requireAngle || (requireAngle && checkAngle))
	//			{
	//				rotate = false;
	//				nextAction.SetInRange(true);
	//				agentMemory.Navigation.TargetReached(nextAction.target);
	//				previousDestination = currentDestination;
	//				agent.isStopped = false;
	//				return true;
	//			}
	//		}

	//		return false;
	//	}
	//	else
	//	{
	//		agentMemory.Navigation.InvalidateTarget();
	//		return false;
	//	}
	//}

	private bool CheckAngle(GameObject target)
	{
		float angle = Utilities.GetAngle(gameObject, target);
		return angle < 5;
	}

	private void Update()
	{
		//if (rotate)
		//{
		//	Vector3 dir = currentDestination - transform.position;
		//	dir.y = 0;//This allows the object to only rotate on its y axis
		//	if (!dir.Equals(Vector3.zero))
		//	{
		//		Quaternion rot = Quaternion.LookRotation(dir);
		//		transform.rotation = Quaternion.Lerp(transform.rotation, rot, 5f * Time.deltaTime);
		//	}
		//}
	}

	public Dictionary<string, bool> GetWorldState()
	{
		return agentMemory.GetWorldState();
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
