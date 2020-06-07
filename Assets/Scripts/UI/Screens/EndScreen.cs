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

    private void Awake()
    {
        startMenuButton.onClick.AddListener(OnStartMenuClicked);
        playAgainButton.onClick.AddListener(OnPlayAgainClicked);
    }
    private void OnDestroy()
    {
        startMenuButton.onClick.RemoveListener(OnStartMenuClicked);
        playAgainButton.onClick.RemoveListener(OnPlayAgainClicked);
    }
    private new void OnEnable()
    {
        base.OnEnable();
        
        SetTitle();
    }

    private void SetTitle()
    {
        Team winningTeam = GameManager.Instance.WinningTeamId;
        MatchConfiguration config = GameManager.Instance.MatchConfiguration;

        if (winningTeam != null)
        {
            title.text = winningTeam.TeamName + " Won";
        }
        else
        {
            title.text = "No Winner";
        }
    }

    private void OnStartMenuClicked()
    {
        GameManager.Instance.ExitMatch();
    }
    private void OnPlayAgainClicked()
    {
        GameManager.Instance.RestartMatch();
    }
    public override void OnBackPressed()
    {
        GameManager.Instance.ExitMatch();
    }
}
