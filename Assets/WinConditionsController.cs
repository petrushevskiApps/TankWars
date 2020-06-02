using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinConditionsController : MonoBehaviour
{
    private Coroutine MatchWinCondition;

    private void Setup(MatchType matchType)
    {
        switch(matchType)
        {
            case MatchType.Deathmatch:
                SetupDeathmatch();
                break;
            case MatchType.TeamVsTeam:
                SetupTeamVsTeam();
                break;
            default:
                break;
        }
    }

    private void SetupTeamVsTeam()
    {
        MatchWinCondition = StartCoroutine(CheckTeams());
    }

    IEnumerator CheckTeams()
    {
        while(true)
        {
            
        }
    }
    private void CheckActiveTeams()
    {
        
    }
    private void SetupDeathmatch()
    {
        throw new NotImplementedException();
    }
}
