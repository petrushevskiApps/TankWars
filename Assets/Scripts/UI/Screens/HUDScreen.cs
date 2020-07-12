using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class HUDScreen : UIScreen
{
    [SerializeField] private List<UITeamScore> teamScoresLeft = new List<UITeamScore>();
    [SerializeField] private List<UITeamScore> teamScoresRight = new List<UITeamScore>();
    private new void OnEnable()
    {
        base.OnEnable();
        SetupScoreCards();
    }

    private void SetupScoreCards()
    {
        List<Team> teams = GameManager.Instance.AgentsController.MatchTeams;
        int leftCount = Mathf.CeilToInt(teams.Count / 2f);
        int rightCount = teams.Count - leftCount;
        int teamIndex = 0;

        for (int i = 0; i < leftCount; i++ ,teamIndex++)
        {
            teamScoresLeft[i].Initialized(teams[teamIndex]);
            teamScoresLeft[i].gameObject.SetActive(true);
        }
        for(int i=0; i < rightCount; i++,teamIndex++)
        {
            teamScoresRight[i].Initialized(teams[teamIndex]);
            teamScoresRight[i].gameObject.SetActive(true);
        }
    }
    private new void  OnDisable()
    {
        base.OnDisable();
        teamScoresLeft.ForEach(team => team.gameObject.SetActive(false));
        teamScoresRight.ForEach(team => team.gameObject.SetActive(false));
    }

    
    public override void OnBackPressed()
    {
        GameManager.Instance.PauseGame();
        UIController.Instance.ShowScreen<PauseScreen>();
    }
}
