using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameModes gameModes;

    public static GameManager Instance;

    private float savedTimeScale = 1;
    public int WinningTeamId { get; private set; }

    [HideInInspector]
    public UnityEvent OnMatchEnd = new UnityEvent();

    public List<List<Agent>> Teams
    {
        get => AgentsController?.GetTeamsList();
    }

    public AgentsController AgentsController { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        AgentsController = GetComponent<AgentsController>();
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

    private MatchConfiguration configuration;

    public void StartMatch(MatchConfiguration matchConfiguration = null)
    {
        if(matchConfiguration != null)
        {
            configuration = matchConfiguration;
        }
        
        if(configuration != null)
        {
            SetupMatch();
        }
        else
        {
            Debug.LogError("Match Configuration is NULL!");
        }
    }
    private void SetupMatch()
    {
        WinningTeamId = 0;
        AgentsController.Setup(configuration);
        AgentsController.OnMatchFinished.AddListener(MatchEnded);

        CameraController.Instance.GameCamera(configuration.CameraMode);
        UIController.Instance.ShowScreen<HUDScreen>();
    }
    public void MatchEnded(int winnerId)
    {
        WinningTeamId = winnerId;
        UIController.Instance.ShowScreen<EndScreen>();
        OnMatchEnd.Invoke();
    }

    public void MatchExited()
    {
        ShowMenu();
        OnMatchEnd.Invoke();
    }

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
        else
        {
            return null;
        }
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
}
