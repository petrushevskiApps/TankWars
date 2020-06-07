using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class HUDScreen : UIScreen
{
    
    [SerializeField] private List<UITeamScore> teamScores;

    private new void OnEnable()
    {
        base.OnEnable();

        for(int i=0; i < teamScores.Count; i++)
        {
            teamScores[i].Initialized(GameManager.Instance.AgentsController.MatchTeams[i]);
        }
    }

    public override void OnBackPressed()
    {
        GameManager.Instance.PauseGame();
        UIController.Instance.ShowScreen<PauseScreen>();
    }
}
