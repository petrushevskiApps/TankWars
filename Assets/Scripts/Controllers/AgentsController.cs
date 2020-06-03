using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class AgentsController : MonoBehaviour
{
    public Agent PlayerAgent { get; private set; }

    [SerializeField] private List<Team> teams = new List<Team>();
    
    [Header("Configurations")]
    [SerializeField] private SpawnLocationsController spawnLocations;
    [SerializeField] private TeamColors teamColors;
    [SerializeField] private NPCNames npcNames;

    [Header("Prefabs")]
    [SerializeField] private GameObject aiPrefab;
    [SerializeField] private GameObject playerPrefab;

    public MatchCompletedEvent OneTeamLeft = new MatchCompletedEvent();

    public int PlayerTeamId { get; private set; } = -1;

    private void Awake()
    {
        GameManager.OnMatchSetup.AddListener(SetupController);
        GameManager.OnMatchEnded.AddListener(ResetController);
    }
    private void OnDestroy()
    {
        GameManager.OnMatchSetup.RemoveListener(SetupController);
        GameManager.OnMatchEnded.RemoveListener(ResetController);
    }


    private void SetupController(MatchConfiguration configuration)
    {
        SpawnAgents(configuration.teamsConfig);
        ActivateTeams();
    }
    private void ResetController()
    {
        foreach (Team team in teams)
        {
            foreach (Agent agent in team.TeamMembers)
            {
                if (agent != null)
                {
                    Destroy(agent.gameObject);
                }
            }
        }

        teams = new List<Team>();
        PlayerAgent = null;
        PlayerTeamId = -1;
    }

    private void SpawnAgents(List<TeamData> teamsConfig) 
    {
        foreach(TeamData teamData in teamsConfig)
        {
            List<Agent> teamMembers = new List<Agent>();

            if(teamData.isPlayer)
            {
                PlayerAgent = InstantiateAgent(playerPrefab, teamMembers);
                PlayerTeamId = teams.Count;
            }

            for (int i=0; i<teamData.agentsCount; i++)
            {
                InstantiateAgent(aiPrefab, teamMembers);
            }

            InitializeTeam(new Team(teams.Count, "NoName", teamData.isPlayer, teamMembers));
        }
    }

    private void OnTeamEmpty(Team team)
    {
        teams.Remove(team);

        if(teams.Count == 1)
        {
            OneTeamLeft.Invoke(teams[0].TeamID);
        }
    }

    private Agent InstantiateAgent(GameObject prefab, List<Agent> teamMembers)
    {
        GameObject agentObject = Instantiate(prefab, spawnLocations.GetSpawnLocation().position, Quaternion.identity);
        Agent agent = agentObject.GetComponent<Agent>();
        teamMembers.Add(agent);

        return agent;
    }

    private void InitializeTeam(Team team)
    {
        foreach (Agent agent in team.TeamMembers)
        {
            agent.Initialize(team.TeamID, npcNames.GetRandomName(), teamColors.GetTeamColor(team.TeamID), team.TeamMembers);
        }

        team.TeamEmpty.AddListener(OnTeamEmpty);
        teams.Add(team);
    }

    
    private void ActivateTeams()
    {
        foreach (Team team in teams)
        {
            team.ActivateTeamMembers();
        }
    }

    public List<Team> GetTeamsList()
    {
        return teams;
    }


    public Agent GetCameraTargetAgent()
    {
        List<Agent> teamMembers = teams[Random.Range(0, teams.Count)].TeamMembers;
        Agent agent = teamMembers[Random.Range(0, teamMembers.Count)];
        agent.GetComponent<AudioListener>().enabled = true;
        return agent;
    }

    public class MatchCompletedEvent : UnityEvent<int> { }
}
