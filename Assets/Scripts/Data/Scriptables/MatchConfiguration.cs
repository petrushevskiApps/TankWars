using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This Scriptable object contains data for
 * different match configurations. According
 * to its parameters.
 */
[CreateAssetMenu(fileName = "MatchConfiguration", menuName = "Data/MatchConfiguration", order = 1)]
public class MatchConfiguration : ScriptableObject
{
    public CameraMode CameraMode;
    public float matchTime = 5f;

    public List<TeamData> teamsConfig = new List<TeamData>();
}

[System.Serializable]
public class TeamData
{
    public int agentsCount = 0;
    public bool isPlayer = false;
}