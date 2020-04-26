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
	
	bool completed = false;

	private Memory agentMemory;
	private float fireAngle = 40f;

	public EliminateEnemy()
	{
		name = "EliminateEnemy";

		AddPrecondition(StateKeys.ENEMY_DETECTED, true);
		AddPrecondition(StateKeys.HEALTH_AMOUNT, true);
		AddPrecondition(StateKeys.AMMO_AMOUNT, true);

		AddEffect(GoalKeys.ELIMINATE_ENEMY, true);

	}
	private void Start()
	{
		agentMemory = GetComponent<Tank>().agentMemory;
	}

	public override void Reset()
	{
		target = null;
		completed = false;
	}

	public override bool IsActionDone()
	{
		return completed;
	}

	public override bool RequiresInRange()
	{
		if (agentMemory.Enemies.IsAnyDetected())
		{
			target = agentMemory.Enemies.GetDetected();
		}
		
		return true;
	}

	public override bool CheckProceduralPrecondition(GameObject agent)
	{
		return true;
	}
	private bool CheckCurrentState(GameObject agent)
	{
		return agentMemory.Enemies.IsAnyDetected() && agentMemory.IsAmmoAvailable();
	}

	public override void Perform(GameObject agent, Action succes, Action fail)
	{
		Debug.Log($"<color=green> {gameObject.name} Perform Action: {this.name}</color>");
		StartCoroutine(Fire(agent, succes, fail));
	}

	IEnumerator Fire(GameObject agent, Action succes, Action fail)
	{
		string targetKey = target.name;

		while (true)
		{ 
			if (CheckCurrentState(agent))
			{
				if (target != null)
				{
					FireBullet(target);
					yield return new WaitForSeconds(0.5f);
				}
				else
				{
					agentMemory.Enemies.RemoveDetected(target);
					succes.Invoke();
					completed = true;
					break;
				}
			}
			else
			{
				fail.Invoke();
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

		agentMemory.DecreaseAmmo();

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
