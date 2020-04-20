using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class Tank : MonoBehaviour, IGoap
{
	NavMeshAgent agent;
	Vector3 previousDestination;
	Inventory inv;

	[SerializeField] private int teamID = 0;

	public int GetTeamID()
	{
		return teamID;
	}
	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		//inv = GetComponent<Inventory>();
	}

	public HashSet<KeyValuePair<string,object>> GetWorldState () 
	{
		HashSet<KeyValuePair<string,object>> worldData = new HashSet<KeyValuePair<string,object>> ();
		//worldData.Add(new KeyValuePair<string, object>("hasFlour", (inv.flourLevel > 1) ));
		//worldData.Add(new KeyValuePair<string, object>("hasDelivery", (inv.breadLevel >= 4) ));		
		return worldData;
	}


	public HashSet<KeyValuePair<string,object>> CreateGoalState ()
	{
		HashSet<KeyValuePair<string,object>> goal = new HashSet<KeyValuePair<string,object>> ();
		goal.Add(new KeyValuePair<string, object>("patrol", true ));

		return goal;
	}


	public bool MoveAgent(GoapAction nextAction) 
	{
		
		// Agent at destination point
		if(previousDestination == nextAction.target)
		{
			Debug.Log("previousDestination == nextAction.target.transform.position");
			nextAction.SetInRange(true);
			return true;
		}
		
		agent.SetDestination(nextAction.target);

		if (agent.hasPath && agent.remainingDistance < 2) 
		{
			nextAction.SetInRange(true);
			previousDestination = nextAction.target;
			Debug.Log("Destination reached!");
			return true;
		}

		return false;
	}

	void Update()
	{
		//if (agent.hasPath)
		//{
		//	Vector3 toTarget = agent.steeringTarget - this.transform.position;
		//	float turnAngle = Vector3.Angle(this.transform.forward, toTarget);
		//	agent.acceleration = turnAngle * agent.speed;
		//}
	}

	public void PlanFailed (HashSet<KeyValuePair<string, object>> failedGoal)
	{

	}

	public void PlanFound (HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions)
	{

	}

	public void ActionsFinished ()
	{

	}

	public void PlanAborted (GoapAction aborter)
	{

	}
}
