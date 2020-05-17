using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class Player : MonoBehaviour, ICollector, IDestroyable
{
	private string name = "tankName";

	[SerializeField] private UIHealthBar healthBar;
	[SerializeField] private GameObject deathParticles;
	[SerializeField] private RenderController renderController;

	[SerializeField] private int teamID = 0;


	private GameObject particles;
	private bool isDead;


	[SerializeField] private Inventory inventory = new Inventory();



	public PlayerDeath OnAgentDeath = new PlayerDeath();


	protected void Awake()
	{
		SetParticles();
		SetHealthBar();
	}

	public void Initialize(int teamID, string name, Material teamColor)
	{
		this.teamID = teamID;
		this.name = name;
		gameObject.name = name;
		renderController.SetTeamColor(teamColor);
	}

	private void SetParticles()
	{
		particles = Instantiate(deathParticles);
		particles.SetActive(false);
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

		OnAgentDeath.Invoke(gameObject);
		// Turn the tank off.
		Destroy(gameObject);
	}

	public void RegisterOnDestroy(UnityAction<GameObject> OnDestroyAction)
	{
		OnAgentDeath.AddListener(OnDestroyAction);
	}

	public Inventory GetInventory()
	{
		return inventory;
	}

	public void PickableCollected(AmmoPack collected)
	{
		inventory.AddAmmo(10);
	}

	public void PickableCollected(HealthPack collected)
	{
		inventory.IncreaseHealth(100);
	}

	public int GetTeamID()
	{
		return teamID;
	}

	public class PlayerDeath : UnityEvent<GameObject>
	{

	}
}
