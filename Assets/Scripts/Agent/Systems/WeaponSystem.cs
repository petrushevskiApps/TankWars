using Complete;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [SerializeField] private GameObject ammoPrefab;
    [SerializeField] private float fireRange;

	// Reference to the audio source used to 
	// play the shooting audio.
	[SerializeField] private AudioSource shootingAudioSource;

	// Audio that plays when each shot is fired.
	[SerializeField] private AudioClip fireAudioClip;

	[SerializeField] private float fireAngle = 40f;

	private Agent player;

	public void Initialize(Agent player)
	{
		this.player = player;
	}

	public void FireBullet(GameObject enemyTarget)
	{
		// Create an instance of the shell
		GameObject shell = Instantiate(ammoPrefab, transform.position, transform.rotation, World.Instance.shellsParent);
		shell.GetComponent<Shell>().SetOwner(player.name, player.GetTeamID());

		Rigidbody shellBody = shell.GetComponent<Rigidbody>();

		// Set the shell's velocity to the launch force in the fire position's forward direction.
		shellBody.velocity = CalcBallisticVelocityVector(transform.position, enemyTarget.transform.position, fireAngle);
		shell.SetActive(true);

		// Change the clip to the firing clip and play it.
		//shootingAudioSource.clip = fireAudioClip;
		//shootingAudioSource.Play();

		player.GetInventory().DecreaseAmmo();

	}
	
	private Vector3 CalcBallisticVelocityVector(Vector3 source, Vector3 target, float angle)
	{
		Vector3 direction = target - source;
		float h = direction.y;
		direction.y = 0;
		float distance = direction.magnitude + 2f;
		float a = angle * Mathf.Deg2Rad;
		direction.y = distance * Mathf.Tan(a);
		distance += h / Mathf.Tan(a);

		// calculate velocity
		float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
		return velocity * direction.normalized;
	}
}
