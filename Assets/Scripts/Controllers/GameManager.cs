using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    // Events
    public static MatchStartedEvent OnMatchSetup = new MatchStartedEvent();
    public static MatchStartedEvent OnMatchStarted = new MatchStartedEvent();
    public static MatchEvent OnMatchExited = new MatchEvent();
    public static MatchEvent OnMatchEnded = new MatchEvent();

    public Team WinningTeamId { get; private set; } = null;

    public AgentsController AgentsController { get; private set; }
    public MatchTimer MatchTimer { get; private set; }

    public MatchConfiguration MatchConfiguration { get; private set; }

    private float savedTimeScale = 1;
    

    private new void Awake()
    {
        base.Awake();

        AgentsController = GetComponent<AgentsController>();
        MatchTimer = GetComponent<MatchTimer>();

        MatchTimer.OnTimerEnd.AddListener(MatchEnded);
        AgentsController.OneTeamLeft.AddListener(MatchEnded);
    }
    private void OnDestroy()
    {
        MatchTimer.OnTimerEnd.RemoveListener(MatchEnded);
        AgentsController.OneTeamLeft.RemoveListener(MatchEnded);
    }

    void Start()
    {
        ShowMenu();
    }
    
    private void ShowMenu()
    {
        CameraController.Instance.ActivateUICamera();
        UIController.Instance.ShowScreen<StartScreen>();
    }

    public void SetupMatch(MatchConfiguration matchConfiguration = null)
    {
        if (matchConfiguration != null)
        {
            MatchConfiguration = matchConfiguration;
        }

        if (MatchConfiguration != null)
        {
            OnMatchSetup.Invoke(MatchConfiguration);
            StartMatch();
        }
        else
        {
            Debug.LogError("Match Configuration is NULL!");
        }
    }

    private void StartMatch()
    {
        WinningTeamId = null;
        OnMatchStarted.Invoke(MatchConfiguration);

        UIController.Instance.ShowScreen<HUDScreen>();
    }


    // Match win conditions fullfiled
    public void MatchEnded(bool isTimerEnd = false)
    {
        SetWinner();

        StartCoroutine(DelayTimer(()=>
        {
            UIController.Instance.ShowScreen<EndScreen>();
            OnMatchEnded.Invoke();
            PauseGame();

        }, isTimerEnd ? 0f : 2f));
        
    }
    IEnumerator DelayTimer(Action delayedAction, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        delayedAction.Invoke();
    }
    private void SetWinner()
    {
        WinningTeamId = AgentsController.GetWinnerTeam();
    }
    // Restart match without changing match
    // configuration and opening start menu
    public void RestartMatch()
    {
        UnpauseGame();
        OnMatchExited.Invoke();
        SetupMatch();
    }

    // Exit match and open start menu for picking
    // new match configuration
    public void ExitMatch()
    {
        UnpauseGame();
        OnMatchExited.Invoke();
        ShowMenu();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
    }

    public class MatchStartedEvent : UnityEvent<MatchConfiguration> { }
    
    public class MatchEvent : UnityEvent { }
}
