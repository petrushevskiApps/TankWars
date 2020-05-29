using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MatchConfiguration", menuName = "Data/MatchConfiguration", order = 1)]
public class MatchConfiguration : ScriptableObject
{
    public MatchType LevelMode;
    public CameraMode CameraMode;

    public List<Team> teamsConfig = new List<Team>();
}

[System.Serializable]
public class Team
{
    public int teamID = 0;
    public int agentsCount = 0;
    public bool isPlayer = false;
}