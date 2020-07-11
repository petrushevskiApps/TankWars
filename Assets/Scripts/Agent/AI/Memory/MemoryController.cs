using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MemoryController : MonoBehaviour
{
    public DetectedHolder Enemies { get; private set; }

    public DetectedHolder AmmoPacks { get; private set; }
    
    public DetectedHolder HealthPacks { get; private set; }

    public DetectedHolder HidingSpots { get; private set; }

    public bool IsUnderAttack { get; private set; }


    private Dictionary<string, Func<bool>> worldState = new Dictionary<string, Func<bool>>();

    private List<Dictionary<string, bool>> goals = new List<Dictionary<string, bool>>();

    private AIAgent agent;

    private Coroutine UnderAttackCoroutine;
    
    public void Initialize(AIAgent agent)
    {
        this.agent = agent;

        Enemies = new DetectedHolder(agent.gameObject, new EnemyComparator());
        AmmoPacks = new DetectedHolder(agent.gameObject, new DistanceComparator());
        HealthPacks = new DetectedHolder(agent.gameObject, new DistanceComparator());
        HidingSpots = new DetectedHolder(agent.gameObject, new DistanceComparator());

        SetStates();
        SetGoals();

        RegisterEvents();
    }
    private void OnDestroy()
    {
        UnregisterEvents();
    }

    private void SetStates()
    {
        worldState.Add(StateKeys.ENEMY_DETECTED, Enemies.IsAnyValidDetected);
        worldState.Add(StateKeys.ENEMY_KILLED, () => false);

        worldState.Add(StateKeys.HEALTH_FULL, () => agent.Inventory.Health.IsFull);
        worldState.Add(StateKeys.HEALTH_DETECTED, HealthPacks.IsAnyValidDetected);

        worldState.Add(StateKeys.AMMO_FULL, () => agent.Inventory.Ammo.IsFull);
        worldState.Add(StateKeys.AMMO_DETECTED, AmmoPacks.IsAnyValidDetected);

        worldState.Add(StateKeys.HIDING_SPOT_DETECTED, HidingSpots.IsAnyValidDetected);

        worldState.Add(StateKeys.UNDER_ATTACK, () => IsUnderAttack);
    }

    private void SetGoals()
    {
        // Survive
        goals.Add(new Dictionary<string, bool>() 
        {
            { StateKeys.ENEMY_KILLED, true },
            { StateKeys.AMMO_FULL, true },
            { StateKeys.HEALTH_FULL, true },
        });

    }

    private void RegisterEvents()
    {
        agent.Sensors.OnEnemyDetected.AddListener(Enemies.AddDetected);
        agent.Sensors.OnEnemyLost.AddListener(Enemies.RemoveDetected);

        agent.Sensors.OnHidingSpotDetected.AddListener(HidingSpots.AddDetected);
        agent.Sensors.OnHidingSpotLost.AddListener(HidingSpots.RemoveDetected);

        agent.Sensors.OnAmmoPackDetected.AddListener(AmmoPacks.AddDetected);
        agent.Sensors.OnAmmoPackLost.AddListener(AmmoPacks.RevalidateDetected);

        agent.Sensors.OnHealthPackDetected.AddListener(HealthPacks.AddDetected);
        agent.Sensors.OnHealthPackLost.AddListener(HealthPacks.RevalidateDetected);

        agent.Sensors.OnUnderAttack.AddListener(SetIsUnderAttack);

    }

    private void UnregisterEvents()
    {
        agent.Sensors.OnEnemyDetected.RemoveListener(Enemies.AddDetected);
        agent.Sensors.OnEnemyLost.RemoveListener(Enemies.RemoveDetected);

        agent.Sensors.OnHidingSpotDetected.RemoveListener(HidingSpots.AddDetected);
        agent.Sensors.OnHidingSpotLost.RemoveListener(HidingSpots.RemoveDetected);

        agent.Sensors.OnAmmoPackDetected.RemoveListener(AmmoPacks.AddDetected);
        agent.Sensors.OnAmmoPackLost.RemoveListener(AmmoPacks.RevalidateDetected);

        agent.Sensors.OnHealthPackDetected.RemoveListener(HealthPacks.AddDetected);
        agent.Sensors.OnHealthPackLost.RemoveListener(HealthPacks.RevalidateDetected);

        agent.Sensors.OnUnderAttack.RemoveListener(SetIsUnderAttack);
    }


    public Dictionary<string, bool> GetWorldState()
    {
        Dictionary<string, bool> agentState = new Dictionary<string, bool>();

        foreach (KeyValuePair<string, Func<bool>> state in worldState)
        {
            agentState.Add(state.Key, state.Value());
        }

        return agentState;
    }

    public List<Dictionary<string, bool>> GetGoals()
    {
        return goals;
    }

    private void SetIsUnderAttack(GameObject missile)
    {
        IsUnderAttack = true;

        if (UnderAttackCoroutine != null)
        {
            agent.StopCoroutine(UnderAttackCoroutine);
        }

        UnderAttackCoroutine = agent.StartCoroutine(UnderAttackTimer());
    }

    IEnumerator UnderAttackTimer()
    {
        yield return new WaitForSeconds(1f);
        UnderAttackCoroutine = null;
        IsUnderAttack = false;
    }

}
