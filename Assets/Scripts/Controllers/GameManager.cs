using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameModes gameModes;
    [SerializeField] private CameraController CameraController;

    public static GameManager Instance;

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
        UIController.Instance.ShowScreen<StartScreen>();
    }

    public void StartMatch(MatchConfiguration matchConfiguration)
    {
        UIController.Instance.ShowScreen<InGameScreen>();
        AgentsController.SpawnAgents(matchConfiguration.teamsConfig);
        CameraController.Setup(matchConfiguration.CameraMode);
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
