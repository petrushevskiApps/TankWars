using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITeamScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI teamName;
    [SerializeField] private TextMeshProUGUI teamKills;
    [SerializeField] private Image background;

    private Team team;

    
    public void Initialized(Team team)
    {
        this.team = team;
        this.team.OnTeamKill.AddListener(UpdateTeamKills);
        gameObject.SetActive(true);
        SetColor();
        SetName();
        UpdateTeamKills(0);
    }

    private void OnDestroy()
    {
        if(team != null)
        {
            team.OnTeamKill.RemoveListener(UpdateTeamKills);
        }
        
    }
    private void SetName()
    {
        teamName.text = team.TeamName;
    }
    private void SetColor()
    {
        Color color = team.teamColor.color;
        color.a = 0.7f;
        background.color = color;
    }
    private void UpdateTeamKills(int score)
    {
        teamKills.text = score.ToString();
    }
}
