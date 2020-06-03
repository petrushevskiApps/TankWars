using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class AgentsController : MonoBehaviour
{
    
    
    [Header("Configurations")]
    [SerializeField] private SpawnLocationsController spawnLocations;
    [SerializeField] private TeamColors teamColors;
    [SerializeField] private NPCNames agentNames;

    [Header("Prefabs")]
    [SerializeField] private GameObject aiPrefab;
    [SerializeField] private GameObject playerPrefab;

    public MatchCompletedEvent OneTeamLeft = new MatchCompletedEvent();

    public int PlayerTeamId { get; private set; } = -1;
    public Agent PlayerAgent { get; private set; }
    public List<Team> MatchTeams { get; private set; } = new List<Team>();

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
        foreach (Team team in MatchTeams)
        {
            foreach (Agent agent in team.TeamMembers)
            {
                if (agent != null)
                {
                    Destroy(agent.gameObject);
                }
            }
        }

        MatchTeams = new List<Team>();
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
                // Add Player to the team if its player team
                PlayerAgent = InstantiateAgent(playerPrefab, teamMembers);
                PlayerTeamId = MatchTeams.Count;
            }

            for (int i=0; i<teamData.agentsCount; i++)
            {
                // Add agents to team as specified in teamData
                InstantiateAgent(aiPrefab, teamMembers);
            }

            InitializeTeam(new Team(MatchTeams.Count, "NoName", teamData.isPlayer, teamMembers));
        }
    }

    // This event is called when team has no
    // more alive agents. If only one team is
    // left with alive members Match ends.
    private void OnTeamEmpty(Team emptyTeam)
    {
        MatchTeams.Remove(emptyTeam);

        if(MatchTeams.Count == 1)
        {
            OneTeamLeft.Invoke(MatchTeams[0].TeamID);
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
            agent.Initialize(team.TeamID, agentNames.GetRandomName(), teamColors.GetTeamColor(team.TeamID), team.TeamMembers);
        }

        team.TeamEmpty.AddListener(OnTeamEmpty);
        MatchTeams.Add(team);
    }

    // Once all teams are initialized
    // enable spawned agents.
    private void ActivateTeams()
    {
        foreach (Team team in MatchTeams)
        {
            team.ActivateTeamMembers();
        }
    }

    public Agent GetCameraTargetAgent()
    {
        // Random team index
        int teamIndex = Random.Range(0, MatchTeams.Count);
        List<Agent> teamMembers = MatchTeams[teamIndex].TeamMembers;

        // Random agent from team
        int agentIndex = Random.Range(0, teamMembers.Count);
        Agent agent = teamMembers[agentIndex];

        agent.GetComponent<AudioListener>().enabled = true;

        return agent;
    }

    public class MatchCompletedEvent : UnityEvent<int> { }
}
