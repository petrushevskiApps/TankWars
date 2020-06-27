using Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRange;

	[HideInInspector]
	public UnityEvent OnShooting = new UnityEvent();

	private Agent agent;
	private bool fireLock = false;

	public void Initialize(Agent agent)
	{
		this.agent = agent;
	}

	public void FireBullet()
	{
		if(agent.Inventory.Ammo.Amount > 0 && !fireLock)
		{
			// Start Fire Rate Timer 
			StartCoroutine(Timer());

			// Create an instance of the shell
			InstantiateBullet();

			OnShooting.Invoke();

			agent.Inventory.Ammo.Decrease(1);
		}
		
	}

	IEnumerator Timer()
	{
		fireLock = true;
		yield return new WaitForSeconds(0.5f);
		fireLock = false;
	}

	private void InstantiateBullet()
	{
		GameObject shell = Instantiate(bulletPrefab, transform.position, transform.rotation, World.Instance.shellsParent);
		
		shell.GetComponent<Shell>().SetOwner(agent);

		Rigidbody shellBody = shell.GetComponent<Rigidbody>();

		// Set the shell's velocity to the launch force in the fire position's forward direction.
		Vector3 velocity = transform.forward * fireRange;

		shellBody.velocity = velocity;
		
		shell.SetActive(true);
	}
}
