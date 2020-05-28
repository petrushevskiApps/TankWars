using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelConfiguration levelConfig;
    [SerializeField] private CameraController CameraController;

    public static GameManager Instance;

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
        StartMatch();
    }

    private void StartMatch()
    {
        AgentsController.SpawnAgents(levelConfig.teamsConfig);
        CameraController.Setup(levelConfig.cameraMode);
    }
}
