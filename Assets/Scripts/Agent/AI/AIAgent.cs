using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class AIAgent : Agent, IGoap
{

	[Header("AI Controllers")]
	[SerializeField] private MemoryController memory;
	[SerializeField] private SensorController sensors;	

	public new AINavigation Navigation { get => (AINavigation) navigationController; }
	public MemoryController Memory { get => memory; }
	public SensorController Sensors { get => sensors; }

	public override void Initialize(Team team, string name, Material teamColor, int agentId)
	{
		base.Initialize(team, name, teamColor, agentId);

		Navigation.Initialize(gameObject, agentId);

		Memory.Initialize(this);

	}

	public void MoveAgent(GoapAction nextAction)
	{
		Navigation.Move(nextAction);
	}

	public Dictionary<string, bool> GetWorldState()
	{
		return Memory.GetWorldState();
	}

	public Dictionary<string, bool> GetGoalState(int index)
	{
		return Memory.GetGoals()[index];
	}

	public int GetGoalsCount()
	{
		return Memory.GetGoals().Count;
	}


	public void PlanFailed(Dictionary<string, bool> failedGoal)
	{

	}

	public void PlanFound(Dictionary<string, bool> goal, Queue<GoapAction> actions)
	{

	}

	public void ActionsFinished()
	{

	}

	public void PlanAborted(GoapAction aborter)
	{

	}

}
