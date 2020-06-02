using Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponSystem : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRange;

	[HideInInspector]
	public UnityEvent OnShooting = new UnityEvent();

	private Agent agent;

	public void Initialize(Agent agent)
	{
		this.agent = agent;
	}

	public void FireBullet()
	{
		if(agent.GetInventory().GetAmmo() > 0)
		{
			// Create an instance of the shell
			InstantiateBullet();

			OnShooting.Invoke();

			agent.GetInventory().DecreaseAmmo();
		}
		
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
