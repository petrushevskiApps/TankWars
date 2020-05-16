using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private LevelConfig levelConfig;
    [SerializeField] private SpawnLocationsController spawnLocations;
    [SerializeField] private TeamColors teamColors;
    [SerializeField] private NPCNames npcNames;

    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private static List<List<NPC>> teamsList;

    private void Awake()
    {
        teamsList = new List<List<NPC>>();
        //npcNames.Setup();

        CreateTeams();
    }

    private void CreateTeams() 
    {
        foreach(Team team in levelConfig.teamsConfig)
        {
            List<NPC> currentTeam = new List<NPC>();

            for(int i=0; i<team.npcCount; i++)
            {
                InstantiateNPC(team.teamID, currentTeam);
            }

            teamsList.Add(currentTeam);
        }
    }

    private void InstantiateNPC(int teamID, List<NPC> teamList)
    {
        GameObject npc = Instantiate(npcPrefab, spawnLocations.GetSpawnLocation().position, Quaternion.identity);
        NPC tankNpc = npc.GetComponent<NPC>();
        
        tankNpc.Initialize(teamID, npcNames.GetRandomName(), teamColors.GetTeamColor(teamID));
        
        teamList.Add(tankNpc);

        npc.SetActive(true);
    }

    public static List<List<NPC>> GetTeamsList()
    {
        return teamsList;
    }
}
