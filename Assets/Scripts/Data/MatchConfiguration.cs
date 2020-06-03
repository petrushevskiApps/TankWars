using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MatchConfiguration", menuName = "Data/MatchConfiguration", order = 1)]
public class MatchConfiguration : ScriptableObject
{
    public MatchType LevelMode;
    public CameraMode CameraMode;

    public List<TeamData> teamsConfig = new List<TeamData>();
}

[System.Serializable]
public class TeamData
{
    public int agentsCount = 0;
    public bool isPlayer = false;
}