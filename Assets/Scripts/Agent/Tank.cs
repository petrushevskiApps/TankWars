using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.Events;
using System;
using GOAP;

public class Tank : MonoBehaviour, IGoap
{
	[SerializeField] private UIHealthBar healthBar;
	[SerializeField] private GameObject deathParticles;

	GameObject particles;
	public bool isDead;

	public Memory agentMemory = new Memory();
	public NavigationSystem Navigation { get; private set; }

	public CommunicationSystem communicationSystem = new CommunicationSystem();

	[SerializeField] private VisionController visionSensor;


	private void Awake()
	{
		Navigation = new NavigationSystem(gameObject);

		agentMemory.Initialize(gameObject);

		agentMemory.AddEvents(visionSensor);

		SetParticles();
		SetHealth();
	}
	private void SetParticles()
	{
		particles = Instantiate(deathParticles);

		particles.SetActive(false);
	}
	private void SetHealth()
	{
		healthBar.Initialize(agentMemory.healthAmount);
	}

	public void TakeDamage(float amount)
	{
		// Reduce current health by the amount of damage done.
		agentMemory.healthAmount -= amount;

		// Change the UI elements appropriately.
		healthBar.SetHealth(agentMemory.healthAmount);

		// If the current health is at or below zero and it has not yet been registered, call OnDeath.
		if (!isDead && agentMemory.healthAmount <= 0f)
		{
			OnDeath();
		}
	}

	private void OnDeath()
	{
		// Set the flag so that this function is only called once.
		isDead = true;

		particles.transform.position = transform.position;

		particles.SetActive(true);
		
		// Turn the tank off.
		Destroy(gameObject);
	}


	private void OnDestroy()
	{
		agentMemory.RemoveEvents(visionSensor);
	}
	
	public void MoveAgent(GoapAction nextAction)
	{
		Navigation.Move(nextAction);
	}

	public Memory GetMemory()
	{
		return agentMemory;
	}

	public NavigationSystem GetNavigation()
	{
		return Navigation;
	}


	public Dictionary<string, bool> GetWorldState()
	{
		return agentMemory.GetWorldState();
	}

	public Dictionary<string, bool> GetGoalState(int index)
	{
		return agentMemory.GetGoals()[index];
	}

	public int GetGoalsCount()
	{
		return agentMemory.GetGoals().Count;
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

	public void ShowMessage(string text)
	{
		communicationSystem.UpdateMessage(text);
	}

	
}
