using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MemorySystem
{
    
    public float shootingRange = 10;

    public Enemies Enemies { get; private set; }

    public AmmoPacks AmmoPacks { get; private set; }
    
    public HealthPacks HealthPacks { get; private set; }

    public HidingSpots HidingSpots { get; private set; }

    private Dictionary<string, Func<bool>> worldState = new Dictionary<string, Func<bool>>();

    private List<Dictionary<string, bool>> goals = new List<Dictionary<string, bool>>();

    private Player parent;

    public void Initialize(Player parent)
    {
        this.parent = parent;

        Enemies = new Enemies(parent.gameObject);
        AmmoPacks = new AmmoPacks(parent.gameObject);
        HealthPacks = new HealthPacks(parent.gameObject);
        HidingSpots = new HidingSpots(parent.gameObject);

        SetStates();
        SetGoals();
    }

    private void SetStates()
    {
        worldState.Add(StateKeys.ENEMY_DETECTED, Enemies.IsAnyValidDetected);
        worldState.Add(StateKeys.IN_SHOOTING_RANGE, () => Enemies.InShootingRange(shootingRange));

        worldState.Add(StateKeys.HEALTH_AMOUNT, parent.GetInventory().IsHealthAvailable);
        worldState.Add(StateKeys.HEALTH_DETECTED, HealthPacks.IsAnyValidDetected);

        worldState.Add(StateKeys.AMMO_AMOUNT, parent.GetInventory().IsAmmoAvailable);
        worldState.Add(StateKeys.AMMO_DETECTED, AmmoPacks.IsAnyValidDetected);

        worldState.Add(StateKeys.HIDING_SPOT_DETECTED, HidingSpots.IsAnyValidDetected);
    }

    private void SetGoals()
    {
        // Survive
        goals.Add(new Dictionary<string, bool>() 
        {
            { StateKeys.ENEMY_DETECTED, false },
            { StateKeys.AMMO_AMOUNT, true },
            { StateKeys.HEALTH_AMOUNT, true }, 
        });

        goals.Add(new Dictionary<string, bool>()
        {
            { StateKeys.HEALTH_AMOUNT, true },
        });

        goals.Add(new Dictionary<string, bool>()
        {
            { StateKeys.AMMO_AMOUNT, true },
        });

        goals.Add(new Dictionary<string, bool>()
        {
            { StateKeys.PATROL, true },
        });
        
    }

    public void RegisterEvents(PerceptorSystem perceptor)
    {
        perceptor.OnEnemyDetected.AddListener(Enemies.AddDetected);
        perceptor.OnEnemyLost.AddListener(Enemies.RemoveDetected);
        perceptor.OnAmmoPackDetected.AddListener(AmmoPacks.AddDetected);
        perceptor.OnHealthPackLost.AddListener(AmmoPacks.RemoveDetected);
        perceptor.OnHealthPackDetected.AddListener(HealthPacks.AddDetected);
        perceptor.OnHealthPackLost.AddListener(HealthPacks.RemoveDetected);
        perceptor.OnHiddingSpotDetected.AddListener(HidingSpots.AddDetected);

    }

    public void RemoveEvents(PerceptorSystem perceptor)
    {
        perceptor.OnEnemyDetected.RemoveListener(Enemies.AddDetected);
        perceptor.OnEnemyLost.RemoveListener(Enemies.RemoveDetected);
        perceptor.OnAmmoPackDetected.RemoveListener(AmmoPacks.AddDetected);
        perceptor.OnHealthPackLost.RemoveListener(AmmoPacks.RemoveDetected);
        perceptor.OnHealthPackDetected.RemoveListener(HealthPacks.AddDetected);
        perceptor.OnHealthPackLost.RemoveListener(HealthPacks.RemoveDetected);
        perceptor.OnHiddingSpotDetected.RemoveListener(HidingSpots.AddDetected);
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

    

    
}
