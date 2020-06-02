using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AgentsController : MonoBehaviour
{
    [SerializeField] private List<List<Agent>> teams = new List<List<Agent>>();
    [SerializeField] private Agent playerAgent;

    [Header("Configurations")]
    [SerializeField] private SpawnLocationsController spawnLocations;
    [SerializeField] private TeamColors teamColors;
    [SerializeField] private NPCNames npcNames;

    [Header("Prefabs")]
    [SerializeField] private GameObject aiPrefab;
    [SerializeField] private GameObject playerPrefab;

    public MatchCompletedEvent OnMatchFinished = new MatchCompletedEvent();
    private List<int> agentsInTeam = new List<int>();

    public int PlayerTeamId { get; private set; } = 0;

    private void Awake()
    {
        GameManager.OnMatchSetup.AddListener(Setup);
        GameManager.OnMatchEnded.AddListener(ResetController);
    }
    private void OnDestroy()
    {
        GameManager.OnMatchSetup.RemoveListener(Setup);
        GameManager.OnMatchEnded.RemoveListener(ResetController);
    }


    private void Setup(MatchConfiguration configuration)
    {
        SpawnAgents(configuration.teamsConfig);
        SetAudioListener();
    }
    private void SpawnAgents(List<Team> teamsConfig) 
    {
        foreach(Team team in teamsConfig)
        {
            List<Agent> currentTeam = new List<Agent>();

            if(team.isPlayer)
            {
                GameObject player = InstantiateAgent(playerPrefab, currentTeam);
                playerAgent = player.GetComponent<Agent>();
                PlayerTeamId = playerAgent.GetTeamID() + 1;
            }
            for (int i=0; i<team.agentsCount; i++)
            {
                InstantiateAgent(aiPrefab, currentTeam);
            }

            agentsInTeam.Add(currentTeam.Count);
            teams.Add(currentTeam);
            
            InitializeAgents(team.teamID, currentTeam);
        }
        ActivateAgents();
    }

    private GameObject InstantiateAgent(GameObject prefab, List<Agent> teamList)
    {
        GameObject agentObject = Instantiate(prefab, spawnLocations.GetSpawnLocation().position, Quaternion.identity);
        agentObject.GetComponent<IDestroyable>().RegisterOnDestroy(OnAgentDestroyed);
        teamList.Add(agentObject.GetComponent<Agent>());

        return agentObject;
    }

    private void OnAgentDestroyed(GameObject agent)
    {
        int id = agent.GetComponent<Agent>().GetTeamID();

        agentsInTeam[id]--;

        if(agentsInTeam[id] == 0)
        {
            OnMatchFinished.Invoke(id);
        }
    }

    private void ResetController()
    {
        foreach(List<Agent> team in teams)
        {
            foreach(Agent agent in team)
            {
                if(agent != null)
                {
                    Destroy(agent.gameObject);
                }
            }
        }

        agentsInTeam = new List<int>();
        teams = new List<List<Agent>>();
        playerAgent = null;
    }

    private void InitializeAgents(int teamID, List<Agent> teamList)
    {
        foreach(Agent agent in teamList)
        {
            agent.Initialize(teamID, npcNames.GetRandomName(), teamColors.GetTeamColor(teamID), teamList);
        }
    }

    private void ActivateAgents()
    {
        foreach (List<Agent> team in teams)
        {
            foreach(Agent agent in team)
            {
                agent.gameObject.SetActive(true);
                agent.gameObject.GetComponent<AudioListener>().enabled = false;
            }
        }
    }

    public Agent GetPlayer()
    {
        return playerAgent;
    }

    public List<List<Agent>> GetTeamsList()
    {
        return teams;
    }


    private void SetAudioListener()
    {
        if (playerAgent != null)
        {
            playerAgent.GetComponent<AudioListener>().enabled = true;
        }
    }

    public Agent GetCameraTargetAgent()
    {
        List<Agent> team = teams[UnityEngine.Random.Range(0, teams.Count)];
        Agent agent = team[UnityEngine.Random.Range(0, team.Count)];
        agent.GetComponent<AudioListener>().enabled = true;
        return agent;
    }

    public class MatchCompletedEvent : UnityEvent<int>
    {

    }
}
