using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using System.Collections;

public abstract class Agent : MonoBehaviour, IDestroyable
{
	//Events
	public PlayerDeath OnAgentDeath = new PlayerDeath();

	[Header("Agent Controllers")]
	[SerializeField] protected CollectController collectController;
	[SerializeField] protected WeaponController weaponController;
	[SerializeField] protected NavigationController navigationController;

	[Header("Agent Systems")]
	[SerializeField] protected InventorySystem inventorySystem;
	[SerializeField] private VisualSystem visualSystem;
	[SerializeField] private AudioSystem audioSystem;


	public CollectController Collector { get => collectController; }
	public WeaponController Weapon { get => weaponController; }
	public InventorySystem Inventory { get => inventorySystem; }
	public VisualSystem VisualSystem { get => visualSystem; }
	public AudioSystem AudioSystem { get => audioSystem; }
	public NavigationController Navigation { get => navigationController; }

	public Team Team { get; private set; }
	public string AgentName { get; private set; } = "tankName";
	public bool IsShieldOn { get; private set; } = false;

	private bool isDead;
	protected int agentId;

	private Coroutine ShieldTimer;

	protected void Awake()
	{
		Weapon.Initialize(this);
		Collector.Initialize(Inventory);
	}


	public virtual void Initialize(Team team, string name, Material teamColor, int agentId)
	{
		Team = team;
		AgentName = name;
		gameObject.name = name;
		this.agentId = agentId;
		visualSystem.Initialize(this, teamColor);
	}

	public void TakeDamage(float amount, Agent owner)
	{
		if(!IsShieldOn)
		{
			// Reduce current health by the amount of damage done.
			inventorySystem.Health.Decrease(amount);
		}

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

		visualSystem.InstantiateDestroyed();

		Destroy(gameObject);
	}
	

	public void RegisterOnDestroy(UnityAction<GameObject> OnDestroyAction)
	{
		OnAgentDeath.AddListener(OnDestroyAction);
	}
	public void UnregisterOnDestroy(UnityAction<GameObject> OnDestroyAction)
	{
		OnAgentDeath.RemoveListener(OnDestroyAction);
	}
	

	public void ToggleShield()
	{
		if(IsShieldOn)
		{
			IsShieldOn = false;
			visualSystem.Renderer.HideShield();
			
			if(ShieldTimer != null)
			{
				StopCoroutine(ShieldTimer);
				ShieldTimer = null;
			}
			Inventory.Shield.Refill();
		}
		else
		{
			IsShieldOn = true;
			visualSystem.Renderer.ShowShield();
			ShieldTimer = StartCoroutine(ShieldToggleTimer());
		}
	}

	IEnumerator ShieldToggleTimer()
	{
		Inventory.Shield.Use();
		yield return new WaitUntil(() => Inventory.Shield.Amount <= 0);
		ToggleShield();
		Inventory.Shield.Refill();
	}

	public void BoostOn()
	{
		navigationController.StartBoosting(this);
	}

	public void BoostOff()
	{
		navigationController.StopBoosting(this);
	}



	public class PlayerDeath : UnityEvent<GameObject> { }
}
