using System;
using System.Collections.Generic;
using UnityEngine;

public class AgentsController : MonoBehaviour
{
    [SerializeField] private List<List<Agent>> teams = new List<List<Agent>>();
    [SerializeField] private List<Agent> players = new List<Agent>();

    [SerializeField] private SpawnLocationsController spawnLocations;
    
    [SerializeField] private TeamColors teamColors;
    [SerializeField] private NPCNames npcNames;

    [SerializeField] private GameObject aiPrefab;
    [SerializeField] private GameObject playerPrefab;


    public void SpawnAgents(TeamsConfig levelConfig) 
    {
        foreach(Team team in levelConfig.teamsConfig)
        {
            List<Agent> currentTeam = new List<Agent>();

            for (int i = 0; i < team.playerCount; i++)
            {
                GameObject player = InstantiatePlayer(currentTeam);
                players.Add(player.GetComponent<Agent>());
            }

            for (int i=0; i<team.npcCount; i++)
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

    private void InitializeAgents(int teamID, List<Agent> teamList)
    {
        foreach(Agent agent in teamList)
        {
            agent.Initialize(teamID, npcNames.GetRandomName(), teamColors.GetTeamColor(teamID), teamList);
        }
    }

    public void ActivateAgents()
    {
        foreach (List<Agent> team in teams)
        {
            foreach(Agent agent in team)
            {
                agent.gameObject.SetActive(true);
            }
        }
    }

    public List<Agent> GetPlayers()
    {
        return players;
    }
    public Agent GetPlayer()
    {
        return players[0];
    }

    public List<List<Agent>> GetTeamsList()
    {
        return teams;
    }

    
    public Agent GetRandomAgent()
    {
        List<Agent> team = teams[UnityEngine.Random.Range(0, teams.Count)];
        return team[UnityEngine.Random.Range(0, team.Count)];
    }
}
