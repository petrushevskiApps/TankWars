using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameModes gameModes;

    public static GameManager Instance;

    private float savedTimeScale = 1;

    public UnityEvent OnMatchEnd = new UnityEvent();

    public List<List<Agent>> Teams
    {
        get => AgentsController?.GetTeamsList();
    }

    public AgentsController AgentsController { get; private set; }
    public InputController InputController { get; private set; }

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
        InputController = GetComponent<InputController>();
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
    public void PauseGame()
    {
        savedTimeScale = Time.timeScale;
        Time.timeScale = 0;
    }

    public void UnpauseGame()
    {
        Time.timeScale = savedTimeScale;
    }

    public void StartMatch(MatchConfiguration matchConfiguration)
    {
        AgentsController.Setup(matchConfiguration);
        CameraController.Instance.GameCamera(matchConfiguration.CameraMode);
        UIController.Instance.ShowScreen<HUDScreen>();
    }
    public void EndMatch()
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
}
