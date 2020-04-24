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

    public int ammoCount = 10;
    public int specialAmmo = 1;
    public int healthCount = 100;

    public Dictionary<string, GameObject>  enemiesDetected = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> ammoDetected = new Dictionary<string, GameObject>();

    private Dictionary<string, Func<bool>> worldState = new Dictionary<string, Func<bool>>();
    private Dictionary<string, bool> agentGoals = new Dictionary<string, bool>();

    public Memory()
    {
        worldState.Add(StateKeys.ENEMY_DETECTED, EnemiesDetected);
        worldState.Add(StateKeys.HEALTH_AMOUNT, () => healthCount > 30);
        worldState.Add(StateKeys.AMMO_AMOUNT, HaveAmmo);
        worldState.Add(StateKeys.AMMO_DETECTED, IsAmmoDetected);
        worldState.Add(StateKeys.AMMO_SPECIAL_AMOUNT, () => specialAmmo > 0);

        agentGoals.Add(GoalKeys.PATROL, true);
        agentGoals.Add(GoalKeys.ELIMINATE_ENEMY, true);
    }

    public bool IsAmmoDetected()
    {
        return ammoDetected.Count() > 0;
    }

    public void RemoveDetectedAmmo(GameObject target)
    {
        if (ammoDetected.ContainsKey(target.name))
        {
            ammoDetected.Remove(target.name);
            Debug.Log("<color=red>InternalState::Detected Ammo Removed | Count: " + ammoDetected.Count + "</color>");
        }
    }

    public void AddAmmo()
    {
        ammoCount += 10;
    }

    public GameObject GetAmmoLocation()
    {
        if (IsAmmoDetected())
        {
            KeyValuePair<string, GameObject> ammoPack = ammoDetected.FirstOrDefault();

            if (ammoPack.Value != null)
            {
                return ammoPack.Value;
            }
            else
            {
                ammoDetected.Remove(ammoPack.Key);
            }
        }
        return null;
    }

    public void AddDetectedAmmoPack(GameObject pack)
    {
        if (!ammoDetected.ContainsKey(pack.name))
        {
            ammoDetected.Add(pack.name, pack);
            Debug.Log("<color=green>InternalState::Ammo Pack Added</color>");
        }
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

    public bool EnemiesDetected()
    {
        //Debug.Log("<color=green>InternalState::Enemies Detected:" + enemiesDetected.Count + "</color>");
        return enemiesDetected.Count > 0;
    }

    public bool HaveAmmo()
    {
        return ammoCount > 0;
    }

    public GameObject GetEnemy()
    {
        if(enemiesDetected.Count > 0)
        {
            KeyValuePair<string,GameObject> enemy = enemiesDetected.FirstOrDefault();

            if(enemy.Value != null)
            {
                return enemy.Value;
            }
            else
            {
                enemiesDetected.Remove(enemy.Key);
            }
        }
        return null;
    }
    public void AddDetectedEnemy(GameObject enemy)
    {
        if(!enemiesDetected.ContainsKey(enemy.name))
        {
            enemiesDetected.Add(enemy.name, enemy);
            Debug.Log("<color=green>InternalState::Enemy Added</color>");
        }
    }
    public void RemoveDetectedEnemy(string enemyKey)
    {
        if (enemiesDetected.ContainsKey(enemyKey))
        {
            enemiesDetected.Remove(enemyKey);
            Debug.Log("<color=red>InternalState::Enemy Removed | Count: " + enemiesDetected.Count + "</color>");
        }
    }

    public void DecreaseAmmo()
    {
        ammoCount--;
    }
}
