using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class Agent : MonoBehaviour, IDestroyable
{
	//Events
	public PlayerDeath OnAgentDeath = new PlayerDeath();

	[Header("Agent Controllers")]
	[SerializeField] protected CollectController collectController;
	[SerializeField] protected WeaponController weaponController;

	[Header("Agent Systems")]
	[SerializeField] protected InventorySystem inventorySystem;
	[SerializeField] private VisualSystem visualSystem;

	public CollectController Collector { get => collectController; }
	public WeaponController Weapon { get => weaponController; }
	public InventorySystem Inventory { get => inventorySystem; }
	public VisualSystem VisualSystem { get => visualSystem; }

	public Team Team { get; private set; }
	public string AgentName { get; private set; } = "tankName";
	

	private bool isDead;

	protected void Awake()
	{
		Weapon.Initialize(this);
		Collector.Initialize(Inventory);
	}


	public virtual void Initialize(Team team, string name, Material teamColor)
	{
		Team = team;
		AgentName = name;
		gameObject.name = name;
		visualSystem.Setup(this, teamColor);
	}

	public void TakeDamage(float amount, Agent owner)
	{
		// Reduce current health by the amount of damage done.
		inventorySystem.Health.Decrease(amount);

		// If the current health is at or below zero
		// and it has not yet been registered, call OnDeath.
		if (!isDead && inventorySystem.Health.Amount <= 0f)
		{
			owner.Team.IncreaseTeamKills();
			OnDeath();
		}
	}

	private void OnDeath()
	{
		// Set the flag so that this function is only called once.
		isDead = true;

		OnAgentDeath.Invoke(gameObject);
		
		Destroy(gameObject);
	}

	public void RegisterOnDestroy(UnityAction<GameObject> OnDestroyAction)
	{
		OnAgentDeath.AddListener(OnDestroyAction);
	}

	public class PlayerDeath : UnityEvent<GameObject> { }
}
