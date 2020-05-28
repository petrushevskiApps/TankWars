using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamsConfig", menuName = "Data/TeamsConfig", order = 1)]
public class TeamsConfig : ScriptableObject
{
    [SerializeField] LevelModes mode = LevelModes.Simulation;
    
    public List<Team> teamsConfig = new List<Team>();


    public LevelModes GetLevelMode()
    {
        return mode;
    }
}

[System.Serializable]
public class Team
{
    public int teamID = 0;
    public int npcCount = 0;
    public int playerCount = 0;
}
