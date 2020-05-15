using Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliminateEnemy : GoapAction
{
	// Prefab of the shell.
	public GameObject bullet;

	// A child of the tank where the shells are spawned.
	public Transform turretTransform;           

	// Reference to the audio source used to 
	// play the shooting audio.
	public AudioSource shootingAudioSource;         

	// Audio that plays when each shot is fired.
	public AudioClip fireAudioClip;                
	
	private IGoap agent;
	private Memory agentMemory;
	private NavigationSystem agentNavigation;

	private float fireAngle = 40f;

	public EliminateEnemy()
	{
		actionName = "EliminateEnemy";

		AddPrecondition(StateKeys.ENEMY_DETECTED, true);
		AddPrecondition(StateKeys.IN_SHOOTING_RANGE, true);
		AddPrecondition(StateKeys.AMMO_AMOUNT, true);
		AddPrecondition(StateKeys.HEALTH_AMOUNT, true);

		AddEffect(StateKeys.ENEMY_DETECTED, false);

	}
	private void Start()
	{
		agent = GetComponent<IGoap>();
		agentMemory = agent.GetMemory();
		agentNavigation = agent.GetNavigation();
	}

	public override void ResetAction()
	{
		base.ResetAction();
	}

	public override void SetActionTarget()
	{
		if (agentMemory.Enemies.IsAnyValidDetected())
		{
			target = agentMemory.Enemies.GetDetected();
			agentNavigation.SetTarget(target);
		} 
	}

	public override bool CheckPreconditions(GameObject agentGO)
	{
		return agentMemory.Enemies.IsAnyValidDetected() && agent.GetInventory().IsAmmoAvailable();
	}

	public override void EnterAction(Action success, Action fail)
	{
		actionCompleted = success;
		actionFailed = fail;
		SetActionTarget();
	}

	public override void ExecuteAction(GameObject agent)
	{
		StartCoroutine(agentNavigation.LookAtTarget(target));
		StartCoroutine(Fire(agent));
	}

	protected override void ExitAction(Action exitAction)
	{
		IsActionDone = true;
		target = null;
		agentNavigation.InvalidateTarget();
		exitAction?.Invoke();
	}

	IEnumerator Fire(GameObject agent)
	{
		while (true)
		{ 
			if (CheckPreconditions(agent))
			{
				if (target != null)
				{
					FireBullet(target);
					yield return new WaitForSeconds(0.5f);
				}
				else
				{
					agentMemory.Enemies.RemoveDetected(target);
					ExitAction(actionCompleted);
					break;
				}
			}
			else
			{
				ExitAction(actionFailed);
				break;
			}
		}
	}

	private void FireBullet(GameObject enemyTarget)
	{
		// Create an instance of the shell
		GameObject shell = Instantiate(bullet, turretTransform.position, turretTransform.rotation);
		
		shell.GetComponent<ShellExplosion>().SetOwner(gameObject);

		Rigidbody shellBody = shell.GetComponent<Rigidbody>();

		// Set the shell's velocity to the launch force in the fire position's forward direction.
		shellBody.velocity = CalcBallisticVelocityVector(turretTransform.position, enemyTarget.transform.position, fireAngle);
		shell.SetActive(true);
		
		// Change the clip to the firing clip and play it.
		shootingAudioSource.clip = fireAudioClip;
		shootingAudioSource.Play();

		agent.GetInventory().DecreaseAmmo();

	}

	private Vector3 CalcBallisticVelocityVector(Vector3 source, Vector3 target, float angle)
	{
		Vector3 direction = target - source;
		float h = direction.y;
		direction.y = 0;
		float distance = direction.magnitude;
		float a = angle * Mathf.Deg2Rad;
		direction.y = distance * Mathf.Tan(a);
		distance += h / Mathf.Tan(a);

		// calculate velocity
		float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
		return velocity * direction.normalized;
	}
}
