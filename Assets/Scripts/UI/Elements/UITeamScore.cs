using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITeamScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI teamName;
    [SerializeField] private TextMeshProUGUI teamKills;

    private Team team;

    
    public void Initialized(Team team)
    {
        this.team = team;
        this.team.OnTeamKill.AddListener(UpdateTeamKills);
        gameObject.SetActive(true);

        SetNames();
        UpdateTeamKills(0);
    }

    private void OnDestroy()
    {
        if(team != null)
        {
            team.OnTeamKill.RemoveListener(UpdateTeamKills);
        }
        
    }
    private void SetNames()
    {
        teamName.text = team.TeamName;
    }
    private void UpdateTeamKills(int score)
    {
        teamKills.text = score.ToString();
    }
}
