using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Memory
{
    
    public float shootingRange = 10;

    public Enemies Enemies { get; private set; }

    public AmmoPacks AmmoPacks { get; private set; }
    
    public HealthPacks HealthPacks { get; private set; }

    public HidingSpots HidingSpots { get; private set; }

    private Dictionary<string, Func<bool>> worldState = new Dictionary<string, Func<bool>>();
    private Dictionary<string, bool> agentGoal = new Dictionary<string, bool>();

    private List<Dictionary<string, bool>> goals = new List<Dictionary<string, bool>>();

    private Tank parent;

    public void Initialize(Tank parent)
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

    public void AddEvents(VisionController visionSensor)
    {
        visionSensor.EnemyDetectedEvent.AddListener(Enemies.AddDetected);
        visionSensor.EnemyLostEvent.AddListener(Enemies.RemoveDetected);
        visionSensor.AmmoPackDetected.AddListener(AmmoPacks.AddDetected);
        visionSensor.AmmoPackLost.AddListener(AmmoPacks.RemoveDetected);
        visionSensor.HealthPackDetected.AddListener(HealthPacks.AddDetected);
        visionSensor.HealthPackLost.AddListener(HealthPacks.RemoveDetected);
        visionSensor.HiddingSpotDetected.AddListener(HidingSpots.AddDetected);

    }

    public void RemoveEvents(VisionController visionSensor)
    {
        visionSensor.EnemyDetectedEvent.RemoveListener(Enemies.AddDetected);
        visionSensor.EnemyLostEvent.RemoveListener(Enemies.RemoveDetected);
        visionSensor.AmmoPackDetected.RemoveListener(AmmoPacks.AddDetected);
        visionSensor.AmmoPackLost.RemoveListener(AmmoPacks.RemoveDetected);
        visionSensor.HealthPackDetected.RemoveListener(HealthPacks.AddDetected);
        visionSensor.HealthPackLost.RemoveListener(HealthPacks.RemoveDetected);
        visionSensor.HiddingSpotDetected.RemoveListener(HidingSpots.AddDetected);
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
