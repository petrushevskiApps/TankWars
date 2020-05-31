using System;
using System.Collections.Generic;
using UnityEngine;

public class AgentsController : MonoBehaviour
{
    [SerializeField] private List<List<Agent>> teams = new List<List<Agent>>();
    [SerializeField] private Agent playerAgent = new Agent();

    [Header("Configurations")]
    [SerializeField] private SpawnLocationsController spawnLocations;
    [SerializeField] private TeamColors teamColors;
    [SerializeField] private NPCNames npcNames;

    [Header("Prefabs")]
    [SerializeField] private GameObject aiPrefab;
    [SerializeField] private GameObject playerPrefab;

    private void Awake()
    {
        GameManager.Instance.OnMatchEnd.AddListener(ResetController);
    }
    private void OnDestroy()
    {
        GameManager.Instance.OnMatchEnd.RemoveListener(ResetController);
    }
    public void Setup(MatchConfiguration configuration)
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
                GameObject player = InstantiatePlayer(currentTeam);
                playerAgent = player.GetComponent<Agent>();
            }
            for (int i=0; i<team.agentsCount; i++)
            {
                InstantiateAgent(currentTeam);
            }

            teams.Add(currentTeam);
            
            InitializeAgents(team.teamID, currentTeam);
        }
        ActivateAgents();
    }

    

    private GameObject InstantiatePlayer(List<Agent> teamList)
    {
        GameObject player = Instantiate(playerPrefab, spawnLocations.GetSpawnLocation().position, Quaternion.identity);

        teamList.Add(player.GetComponent<Agent>());

        return player;
    }

    private void InstantiateAgent(List<Agent> teamList)
    {
        GameObject agentObject = Instantiate(aiPrefab, spawnLocations.GetSpawnLocation().position, Quaternion.identity);
        
        teamList.Add(agentObject.GetComponent<Agent>());
    }

    private void ResetController()
    {
        foreach(List<Agent> team in teams)
        {
            foreach(Agent agent in team)
            {
                Destroy(agent.gameObject);
            }
        }

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

}
