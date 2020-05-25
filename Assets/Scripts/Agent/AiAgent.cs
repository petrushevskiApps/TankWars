using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class AIAgent : Agent, IGoap
{
	[Header("AI Systems")]

	[SerializeField] private MemorySystem memory = new MemorySystem();

	[SerializeField] private NavigationSystem Navigation { get; set; }

	[SerializeField] private CommunicationSystem communication;

	[SerializeField] private PerceptorSystem perceptor;

	protected void Awake()
	{
		base.Awake();

	}

	public override void Initialize(int teamID, string name, Material teamColor, List<Agent> team)
	{
		base.Initialize(teamID, name, teamColor, team);

		Navigation = new NavigationSystem(gameObject);

		memory.Initialize(this);

		memory.RegisterEvents(perceptor);

		communication.Initialize(this);
	}

	public List<Agent> GetTeamMembers()
	{
		return team;
	}

	private void OnDestroy()
	{
		memory.UnregisterEvents(perceptor);
		Navigation.OnDestroy();
		
	}

	public void MoveAgent(GoapAction nextAction)
	{
		Navigation.Move(nextAction);
	}

	public MemorySystem GetMemory()
	{
		return memory;
	}
	public NavigationSystem GetNavigation()
	{
		return Navigation;
	}
	
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
		return memory.GetWorldState();
	}

	public Dictionary<string, bool> GetGoalState(int index)
	{
		return memory.GetGoals()[index];
	}

	public int GetGoalsCount()
	{
		return memory.GetGoals().Count;
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
