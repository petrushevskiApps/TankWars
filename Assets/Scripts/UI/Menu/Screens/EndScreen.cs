using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : UIScreen
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Button startMenuButton;
    [SerializeField] private Button playAgainButton;

    private int teamID = 0;
    private int playerTeamId = 0;

    private void Awake()
    {
        startMenuButton.onClick.AddListener(OnStartMenuClicked);
        playAgainButton.onClick.AddListener(OnPlayAgainClicked);
    }

    private void OnPlayAgainClicked()
    {
        GameManager.Instance.StartMatch();
    }

    private void OnStartMenuClicked()
    {
        GameManager.Instance.MatchExited();
    }

    private new void OnEnable()
    {
        base.OnEnable();
        teamID = GameManager.Instance.WinningTeamId;
        playerTeamId = GameManager.Instance.AgentsController.PlayerTeamId;
        SetupTitle();
    }

    private void SetupTitle()
    {
        if(teamID == 0)
        {
            title.text = "No Winner";
        }
        else
        {
            if(playerTeamId != 0)
            {
                if (playerTeamId == teamID)
                {
                    title.text = "Your Team Won";
                }
                else
                {
                    title.text = "Your Team Lost";
                }
            }
            else
            {
                title.text = "Team " + teamID + " Won";
            }
        }
    }

    public override void OnBackButton()
    {
        GameManager.Instance.MatchExited();
    }
}
