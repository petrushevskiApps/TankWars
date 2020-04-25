using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Memory
{
    [SerializeField] private int teamID = 0;
    [SerializeField] private int enemyRank = 1;

    public int ammoAmount = 10;
    public int specialAmmo = 1;
    public int healthAmount = 100;

    public Enemies Enemies { get; private set; }
    
    public AmmoPacks AmmoPacks { get; private set; }

    private Dictionary<string, Func<bool>> worldState = new Dictionary<string, Func<bool>>();
    private Dictionary<string, bool> agentGoals = new Dictionary<string, bool>();

    public Memory()
    {
        Enemies = new Enemies();
        AmmoPacks = new AmmoPacks();

        worldState.Add(StateKeys.ENEMY_DETECTED, Enemies.IsAnyDetected);
        worldState.Add(StateKeys.HEALTH_AMOUNT, () => healthAmount > 30);
        worldState.Add(StateKeys.AMMO_AMOUNT, HaveAmmo);
        worldState.Add(StateKeys.AMMO_DETECTED, AmmoPacks.IsAnyDetected);
        worldState.Add(StateKeys.AMMO_SPECIAL_AMOUNT, () => specialAmmo > 0);

        agentGoals.Add(GoalKeys.PATROL, true);
        agentGoals.Add(GoalKeys.ELIMINATE_ENEMY, true);
    }

    public void AddEvents(VisionController visionSensor)
    {
        visionSensor.EnemyDetectedEvent.AddListener(Enemies.AddDetected);
        visionSensor.EnemyLostEvent.AddListener(Enemies.RemoveDetected);
        visionSensor.AmmoPackDetected.AddListener(AmmoPacks.AddDetected);
    }
    public void RemoveEvents(VisionController visionSensor)
    {
        visionSensor.EnemyDetectedEvent.RemoveListener(Enemies.AddDetected);
        visionSensor.EnemyLostEvent.RemoveListener(Enemies.RemoveDetected);
        visionSensor.AmmoPackDetected.RemoveListener(AmmoPacks.AddDetected);
    }

    public Dictionary<string, Func<bool>> GetWorldState()
    {
        return worldState;
    }
    public Dictionary<string, bool> GetGoalState()
    {
        return agentGoals;
    }

    public int GetTeamID()
    {
        return teamID;
    }

    public bool HaveAmmo()
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
