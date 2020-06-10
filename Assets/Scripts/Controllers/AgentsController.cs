using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class AgentsController : MonoBehaviour
{
    [Header("Configurations")]
    [SerializeField] private SpawnLocationsController spawnLocations;
    [SerializeField] private TeamColors teamColors;
    [SerializeField] private Names agentNamesList;
    [SerializeField] private Names teamNamesList;

    [Header("Prefabs")]
    [SerializeField] private GameObject aiPrefab;
    [SerializeField] private GameObject playerPrefab;

    public OneTeamLeftEvent OneTeamLeft = new OneTeamLeftEvent();

    public int PlayerTeamId { get; private set; } = -1;
    public Agent PlayerAgent { get; private set; }
    public List<Team> MatchTeams { get; private set; } = new List<Team>();

    private List<string> teamNames = new List<string>();
    private List<string> agentNames = new List<string>();

    private void Awake()
    {
        GameManager.OnMatchSetup.AddListener(SetupController);
        GameManager.OnMatchExited.AddListener(ResetController);
    }
    private void OnDestroy()
    {
        GameManager.OnMatchSetup.RemoveListener(SetupController);
        GameManager.OnMatchExited.RemoveListener(ResetController);
    }


    private void SetupController(MatchConfiguration configuration)
    {
        Setup(agentNames, agentNamesList);
        Setup(teamNames, teamNamesList);
        SpawnAgents(configuration.teamsConfig);
        ActivateTeams();
    }
    private void ResetController()
    {
        foreach (Team team in MatchTeams)
        {
            foreach (Agent agent in team.Members)
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

    int agentId = 0;

    private void SpawnAgents(List<TeamData> teamsConfig) 
    {
        agentId = 0;
        
        foreach (TeamData teamData in teamsConfig)
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

            InitializeTeam(new Team(MatchTeams.Count, GetRandomName(teamNames), teamData.isPlayer, teamMembers));
        }
    }

    // This event is called when team has no
    // more alive agents. If only one team is
    // left with alive members Match ends.
    private void OnTeamEmpty(Team emptyTeam)
    {
        MatchTeams.Remove(emptyTeam);

        if(MatchTeams.Count == 1) OneTeamLeft.Invoke(false);

    }

    public Team GetWinnerTeam()
    {
        if(MatchTeams.Count == 1)
        {
            return MatchTeams[0];
        }
        else if (MatchTeams.Count >= 2)
        {
            MatchTeams.Sort();

            if(MatchTeams[0].TeamKills == MatchTeams[1].TeamKills)
            {
                return null;
            }
            else return MatchTeams[0];
        }
        else return null;
    }

    private Agent InstantiateAgent(GameObject prefab, List<Agent> teamMembers)
    {
        GameObject agentObject = Instantiate(prefab, spawnLocations.GetSpawnLocation(), Quaternion.identity);
        Agent agent = agentObject.GetComponent<Agent>();
        teamMembers.Add(agent);

        return agent;
    }

    private void InitializeTeam(Team team)
    {
        foreach(Agent agent in team.Members)
        {
            agent.Initialize(team, GetRandomName(agentNames), teamColors.GetTeamColor(team.ID), 0);
        }

        team.OnTeamEmpty.AddListener(OnTeamEmpty);
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

    

    public void Setup(List<string> availableNames, Names namesList)
    {
        availableNames.Clear();

        if (availableNames.Count <= 0)
        {
            namesList.names.ForEach(name => availableNames.Add(name));
        }
    }

    public string GetRandomName(List<string> availableNames)
    {
        if (availableNames.Count > 0)
        {
            int index = Random.Range(0, availableNames.Count);
            string name = availableNames[index];
            availableNames.RemoveAt(index);
            return name;

        }

        return "NoNameAvailable";
    }

    public List<Agent> GetAllAgents()
    {
        List<Agent> allAgents = new List<Agent>();
        MatchTeams.ForEach(team => team.Members.ForEach(agent => allAgents.Add(agent)));
        return allAgents;
    }
    public class OneTeamLeftEvent : UnityEvent<bool> { }
}
