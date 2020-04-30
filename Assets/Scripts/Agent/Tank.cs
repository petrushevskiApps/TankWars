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

	[SerializeField] private VisionController visionSensor; 

	private void Awake()
	{
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
	
	public bool MoveAgent(GoapAction nextAction)
	{
		agentMemory.Navigation.MoveAgent(nextAction);
		return agentMemory.Navigation.IsAgentOnTarget(nextAction);
	}


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
