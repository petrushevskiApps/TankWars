using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class AIAgent : Agent, IGoap
{
	

	public MemorySystem Memory { get; private set; }

	public NavigationSystem Navigation { get; private set; }

	[Header("AI Systems")]

	[SerializeField] private CommunicationSystem communication;

	[SerializeField] private PerceptorSystem perceptor;

	public override void Initialize(Team team, string name, Material teamColor)
	{
		base.Initialize(team, name, teamColor);

		Memory = new MemorySystem();
		Navigation = new NavigationSystem(gameObject);

		Memory.Initialize(this);

		Memory.RegisterEvents(perceptor);

		communication.Initialize(this);
	}

	

	private void OnDestroy()
	{
		Memory.UnregisterEvents(perceptor);
		Navigation.OnDestroy();
		
	}

	public void MoveAgent(GoapAction nextAction)
	{
		Navigation.Move(nextAction);
	}

	//public MemorySystem GetMemory()
	//{
	//	return Memory;
	//}

	
	public PerceptorSystem GetPerceptor()
	{
		return perceptor;
	}
	public CommunicationSystem GetCommunication()
	{
		return communication;
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

	public void ShowMessage(string text)
	{
		
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
