using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Memory
{
    [SerializeField] private int teamID = 0;
    [SerializeField] private int agentRank = 1;

    public int ammoAmount = 10;
    public int specialAmmo = 1;
    public float healthAmount = 100;

    public Enemies Enemies { get; private set; }

    public AmmoPacks AmmoPacks { get; private set; }

    

    private Dictionary<string, Func<bool>> worldState = new Dictionary<string, Func<bool>>();
    private Dictionary<string, bool> agentGoals = new Dictionary<string, bool>();


    public void Initialize(GameObject parent)
    {
        Enemies = new Enemies(parent);
        AmmoPacks = new AmmoPacks(parent);

        SetStates();
        SetGoals();
    }

    private void SetStates()
    {
        worldState.Add(StateKeys.ENEMY_DETECTED, Enemies.IsAnyValidDetected);
        worldState.Add(StateKeys.HEALTH_AMOUNT, () => healthAmount > 30);
        worldState.Add(StateKeys.AMMO_AMOUNT, IsAmmoAvailable);
        worldState.Add(StateKeys.AMMO_DETECTED, AmmoPacks.IsAnyValidDetected);
        worldState.Add(StateKeys.AMMO_SPECIAL_AMOUNT, () => specialAmmo > 0);
    }

    private void SetGoals()
    {
        agentGoals.Add(GoalKeys.SURVIVE, true);
        agentGoals.Add(GoalKeys.PATROL, true);
        agentGoals.Add(GoalKeys.ELIMINATE_ENEMY, true);
    }

    public void AddEvents(VisionController visionSensor)
    {
        visionSensor.EnemyDetectedEvent.AddListener(Enemies.AddDetected);
        visionSensor.EnemyLostEvent.AddListener(Enemies.RemoveDetected);
        visionSensor.AmmoPackDetected.AddListener(AmmoPacks.AddDetected);
        visionSensor.AmmoPackLost.AddListener(AmmoPacks.RemoveDetected);
    }

    public void RemoveEvents(VisionController visionSensor)
    {
        visionSensor.EnemyDetectedEvent.RemoveListener(Enemies.AddDetected);
        visionSensor.EnemyLostEvent.RemoveListener(Enemies.RemoveDetected);
        visionSensor.AmmoPackDetected.RemoveListener(AmmoPacks.AddDetected);
        visionSensor.AmmoPackLost.RemoveListener(AmmoPacks.RemoveDetected);
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
    public Dictionary<string, bool> GetGoalState()
    {
        return agentGoals;
    }

    public int GetTeamID()
    {
        return teamID;
    }

    public bool IsAmmoAvailable()
    {
        return ammoAmount > 0;
    }
    public void IncreaseAmmo()
    {
        ammoAmount += 10;
    }
    public void DecreaseAmmo()
    {
        ammoAmount--;
    }
    
}
