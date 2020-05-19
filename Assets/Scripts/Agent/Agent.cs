using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class Agent : Player, IGoap
{
	[Header("Agent Systems")]

	[SerializeField] private MemorySystem memory = new MemorySystem();

	[SerializeField] private NavigationSystem Navigation { get; set; }

	[SerializeField] private CommunicationSystem communication;

	[SerializeField] private PerceptorSystem perceptor;

	public bool IsUnderAttack { get; private set; }
	private Coroutine UnderAttackCoroutine;

	protected void Awake()
	{
		base.Awake();

		Navigation = new NavigationSystem(gameObject);

		memory.Initialize(this);

		memory.RegisterEvents(perceptor);

		inventory.UnderAttack.AddListener(OnUnderAttack);

	}

	private void OnUnderAttack()
	{
		IsUnderAttack = true;

		if(UnderAttackCoroutine != null)
		{
			// Restart coroutine each attack
			StopCoroutine(UnderAttackCoroutine);
		}
		UnderAttackCoroutine = StartCoroutine(UnderAttackTimer());
	}
	IEnumerator UnderAttackTimer()
	{
		yield return new WaitForSecondsRealtime(2f);
		UnderAttackCoroutine = null;
		IsUnderAttack = false;
	}

	private void OnDestroy()
	{
		memory.RemoveEvents(perceptor);
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
		communication.UpdateMessage(text);
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
