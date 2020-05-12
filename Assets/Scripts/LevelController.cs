using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private LevelConfig levelConfig;
    [SerializeField] private SpawnLocationsController spawnLocations;
    [SerializeField] private TeamColors teamColors;
    [SerializeField] private NPCNames npcNames;

    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private static List<List<Tank>> teamsList;

    private void Awake()
    {
        teamsList = new List<List<Tank>>();
        //npcNames.Setup();

        CreateTeams();
    }

    private void CreateTeams() 
    {
        foreach(Team team in levelConfig.teamsConfig)
        {
            List<Tank> currentTeam = new List<Tank>();

            for(int i=0; i<team.npcCount; i++)
            {
                InstantiateNPC(team.teamID, currentTeam);
            }

            teamsList.Add(currentTeam);
        }
    }

    private void InstantiateNPC(int teamID, List<Tank> teamList)
    {
        GameObject npc = Instantiate(npcPrefab, spawnLocations.GetSpawnLocation().position, Quaternion.identity);
        Tank tankNpc = npc.GetComponent<Tank>();
        
        tankNpc.Initialize(teamID, npcNames.GetRandomName(), teamColors.GetTeamColor(teamID));
        
        teamList.Add(tankNpc);

        npc.SetActive(true);
    }

    public static List<List<Tank>> GetTeamsList()
    {
        return teamsList;
    }
}
