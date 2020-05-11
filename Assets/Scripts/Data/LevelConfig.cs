using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Data/LevelConfig", order = 1)]
public class LevelConfig : ScriptableObject
{
    [SerializeField] LevelModes mode = LevelModes.Simulation;
    
    [SerializeField] List<Team> teams = new List<Team>();


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
