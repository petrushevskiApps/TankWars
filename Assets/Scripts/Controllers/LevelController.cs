using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private LevelConfig levelConfig;
    [SerializeField] private SpawnLocationsController spawnLocations;
    [SerializeField] private TeamColors teamColors;
    [SerializeField] private NPCNames npcNames;

    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private static List<List<Agent>> teamsList;

    private void Awake()
    {
        teamsList = new List<List<Agent>>();
        //npcNames.Setup();

        CreateTeams();
    }

    private void CreateTeams() 
    {
        foreach(Team team in levelConfig.teamsConfig)
        {
            List<Agent> currentTeam = new List<Agent>();

            for(int i=0; i<team.npcCount; i++)
            {
                InstantiateNPC(currentTeam);
            }

            teamsList.Add(currentTeam);
            
            InitializeAgents(team.teamID, currentTeam);
        }
    }

    private void InstantiateNPC(List<Agent> teamList)
    {
        GameObject agentObject = Instantiate(npcPrefab, spawnLocations.GetSpawnLocation().position, Quaternion.identity);
        
        teamList.Add(agentObject.GetComponent<Agent>());

    }

    private void InitializeAgents(int teamID, List<Agent> teamList)
    {
        foreach(Agent agent in teamList)
        {
            agent.Initialize(teamID, npcNames.GetRandomName(), teamColors.GetTeamColor(teamID), teamList);
            agent.gameObject.SetActive(true);
        }
    }

    public static List<List<Agent>> GetTeamsList()
    {
        return teamsList;
    }
}
