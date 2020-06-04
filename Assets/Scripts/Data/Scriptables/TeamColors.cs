using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This Scriptable object contains list of
 * materials to be assigned on each active
 * team in match. So the teams could be different.
 */
[CreateAssetMenu(fileName = "TeamColors", menuName = "Data/TeamColors", order = 1)]
public class TeamColors : ScriptableObject
{
    [SerializeField] List<Material> teamColors = new List<Material>();

    public Material GetTeamColor(int teamId)
    {
        return teamColors[teamId];
    }
}
