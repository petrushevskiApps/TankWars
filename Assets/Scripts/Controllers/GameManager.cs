using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameModes gameModes;

    public int WinningTeamId { get; private set; }
    
    public List<Team> Teams => AgentsController?.GetTeamsList();

    private float savedTimeScale = 1;
    private MatchConfiguration configuration;

    public static MatchStartedEvent OnMatchSetup = new MatchStartedEvent();
    public static MatchStartedEvent OnMatchStarted = new MatchStartedEvent();
    public static MatchEvent OnMatchEnded = new MatchEvent();

    public AgentsController AgentsController { get; private set; }

    private new void Awake()
    {
        base.Awake();

        AgentsController = GetComponent<AgentsController>();
        AgentsController.OneTeamLeft.AddListener(MatchEnded);
    }
    private void OnDestroy()
    {
        AgentsController.OneTeamLeft.RemoveListener(MatchEnded);
    }
    void Start()
    {
        ShowMenu();
    }
    
    private void ShowMenu()
    {
        CameraController.Instance.ToggleUICamera(true);
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

    public void MatchEnded(int winnerId)
    {
        WinningTeamId = winnerId;
        UIController.Instance.ShowScreen<EndScreen>();
        OnMatchEnded.Invoke();
    }

    public void MatchExited()
    {
        ShowMenu();
        OnMatchEnded.Invoke();
    }

    //TODO:: Refactor this
    public List<MatchConfiguration> GetMatchConfigurations(GameModeTypes gameModeType)
    {
        if(gameModeType == GameModeTypes.Simulation)
        {
            return gameModes.simulationConfigs;
        }
        else if(gameModeType == GameModeTypes.Player)
        {
            return gameModes.playerConfigs;
        }
        else return null;
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
