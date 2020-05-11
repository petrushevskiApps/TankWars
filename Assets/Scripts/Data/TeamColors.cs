using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamColors", menuName = "Data/TeamColors", order = 1)]
public class TeamColors : ScriptableObject
{
    [SerializeField] List<Material> teamColors = new List<Material>();

    public Material GetTeamColor(int teamId)
    {
        return teamColors[teamId];
    }
}
