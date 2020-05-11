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
	[SerializeField] private RenderController renderController;

	[SerializeField] private int teamID = 0;
	private string name = "tankName";

	GameObject particles;
	public bool isDead;

	
	public Memory memory = new Memory();

	[SerializeField] private Inventory inventory = new Inventory();

	public NavigationSystem Navigation { get; private set; }

	public CommunicationSystem communicationSystem = new CommunicationSystem();

	[SerializeField] private VisionController visionSensor;


	private void Awake()
	{
		Navigation = new NavigationSystem(gameObject);

		memory.Initialize(this);

		memory.AddEvents(visionSensor);

		SetParticles();
		SetHealthBar();
	}

	private void OnDestroy()
	{
		memory.RemoveEvents(visionSensor);
	}

	private void SetParticles()
	{
		particles = Instantiate(deathParticles);
		particles.SetActive(false);
	}

    public void Initialize(int teamID, string name, Material teamColor)
    {
		this.teamID = teamID;
		this.name = name;
		gameObject.name = name;
		renderController.SetTeamColor(teamColor);
		
    }

    private void SetHealthBar()
	{
		healthBar.Initialize(inventory.GetHealth(), inventory.OnHealthChange);
	}

	public void TakeDamage(float amount)
	{
		// Reduce current health by the amount of damage done.
		
		inventory.DecreaseHealth(amount);

		// If the current health is at or below zero and it has not yet been registered, call OnDeath.
		if (!isDead && inventory.GetHealth() <= 0f)
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


	
	
	public void MoveAgent(GoapAction nextAction)
	{
		Navigation.Move(nextAction);
	}

	public Memory GetMemory()
	{
		return memory;
	}

	public NavigationSystem GetNavigation()
	{
		return Navigation;
	}
	public Inventory GetInventory()
	{
		return inventory;
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

	public int GetTeamID()
	{
		return teamID;
	}
}
