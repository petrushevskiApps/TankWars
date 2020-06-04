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

    public int WinningTeamId { get; private set; }

    public AgentsController AgentsController { get; private set; }

    private float savedTimeScale = 1;
    private MatchConfiguration configuration;

    private new void Awake()
    {
        base.Awake();

        AgentsController = GetComponent<AgentsController>();
        AgentsController.OnWinCondition.AddListener(MatchEnded);
    }
    private void OnDestroy()
    {
        AgentsController.OnWinCondition.RemoveListener(MatchEnded);
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
            configuration = matchConfiguration;
        }

        if (configuration != null)
        {
            OnMatchSetup.Invoke(configuration);
            StartMatch();
        }
        else
        {
            Debug.LogError("Match Configuration is NULL!");
        }
    }

    private void StartMatch()
    {
        WinningTeamId = -1;
        OnMatchStarted.Invoke(configuration);

        UIController.Instance.ShowScreen<HUDScreen>();
    }

    // Match win conditions fullfiled
    public void MatchEnded(int winnerId)
    {
        WinningTeamId = winnerId;
        UIController.Instance.ShowScreen<EndScreen>();
        PauseGame();
    }

    // Restart match without changing match
    // configuration and opening start menu
    public void MatchRestarted()
    {
        UnpauseGame();
        OnMatchExited.Invoke();
        SetupMatch();
    }

    // Exit match and open start menu for picking
    // new match configuration
    public void MatchExited()
    {
        UnpauseGame();
        OnMatchExited.Invoke();
        ShowMenu();
    }

    public void PauseGame()
    {
        savedTimeScale = Time.timeScale;
        Time.timeScale = 0;
    }

    public void UnpauseGame()
    {
        Time.timeScale = savedTimeScale;
    }

    public class MatchStartedEvent : UnityEvent<MatchConfiguration> { }
    
    public class MatchEvent : UnityEvent { }
}
