using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class Agent : MonoBehaviour, ICollector, IDestroyable
{
	//Events
	public PlayerDeath OnAgentDeath = new PlayerDeath();

	[SerializeField] private int teamID = 0;
	public GameObject cameraTracker;

	[Header("Agent Controllers")]
	[SerializeField] private AgentUIController uiController; 
	[SerializeField] private RenderController renderController;

	

	[Header("Agent Systems")]
	[SerializeField] protected Inventory inventory = new Inventory();
	[SerializeField] protected WeaponSystem weapon;
	[SerializeField] protected AgentParticlesSystem particlesSystem;

	protected List<Agent> team = new List<Agent>();

	protected string name = "tankName";

	private bool isDead;

	protected void Awake()
	{
		weapon.Initialize(this);

		uiController.Setup(this);
	}

	private void Start()
	{
		inventory.Initialize();
	}
	private void OnEnable()
	{
		particlesSystem.PlayParticles();
	}
	private void OnDisable()
	{
		particlesSystem.StopParticles();
	}

	public virtual void Initialize(int teamID, string name, Material teamColor, List<Agent> team)
	{
		this.teamID = teamID;
		this.team = team;
		this.name = name;
		gameObject.name = name;
		renderController.SetTeamColor(teamColor);
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
		OnAgentDeath.Invoke(gameObject);

		// Set the flag so that this function is only called once.
		isDead = true;

		renderController.ShowParticles();
		
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
	public WeaponSystem GetWeapon()
	{
		return weapon;
	}
	public void PickableCollected(AmmoPack collected)
	{
		inventory.IncreaseAmmo(collected.amountToCollect);
	}

	public void PickableCollected(HealthPack collected)
	{
		inventory.IncreaseHealth(collected.amountToCollect);
	}

	public int GetTeamID()
	{
		return teamID;
	}
	public string GetName()
	{
		return name;
	}
	public List<Agent> GetTeamMembers()
	{
		return team;
	}

	public class PlayerDeath : UnityEvent<GameObject>
	{

	}
}
