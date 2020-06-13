using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using System.Collections;

public class Agent : MonoBehaviour, IDestroyable
{
	[SerializeField] private GameObject destroyedAgentPrefab;

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
		visualSystem.Setup(this, teamColor);
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

		InstantiateDestroyed();

		Destroy(gameObject);
	}
	private void InstantiateDestroyed()
	{
		GameObject destroyedAgent = Instantiate(destroyedAgentPrefab, transform.position, transform.rotation, World.Instance.destroyedAgents);
		destroyedAgent.name = destroyedAgent.name.Replace("(Clone)", "( " + AgentName + " ) ");
		visualSystem.DropTracker(destroyedAgent.transform);
		destroyedAgent.SetActive(true);
	}
	public void RegisterOnDestroy(UnityAction<GameObject> OnDestroyAction)
	{
		OnAgentDeath.AddListener(OnDestroyAction);
	}

	private Coroutine ShieldTimer;

	protected void ToggleShield()
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

	protected void BoostOn()
	{
		if (Inventory.SpeedBoost.Amount > 0)
		{
			navigationController.BoostSpeed();
			Inventory.SpeedBoost.Use();
		}
		else
		{
			navigationController.ResetSpeed();
		}
	}
	protected void BoostOff()
	{
		navigationController.ResetSpeed();
		Inventory.SpeedBoost.Refill();
	}

	public class PlayerDeath : UnityEvent<GameObject> { }
}
