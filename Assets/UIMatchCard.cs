using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMatchCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI subtitle;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Button startMatchButton;

    private MatchConfiguration matchConfiguration;

    private void Awake()
    {
        startMatchButton.onClick.AddListener(StartMatch);
    }
    private void OnDestroy()
    {
        startMatchButton.onClick.RemoveListener(StartMatch);
    }
    private void StartMatch()
    {
        GameManager.Instance.StartMatch(matchConfiguration);
    }

    public void Setup(MatchConfiguration matchConfiguration)
    {
        this.matchConfiguration = matchConfiguration;
        SetText(title, matchConfiguration.LevelMode.ToString());
        SetText(subtitle, GetSubtitleText());
        SetText(description, GetDescriptionText());
        gameObject.SetActive(true);
    }

    private string GetDescriptionText()
    {
        switch(matchConfiguration.LevelMode)
        {
            case MatchType.Deathmatch:
                return ConstStrings.DeathmatchDescription;
            case MatchType.TeamVsTeam:
                return ConstStrings.TeamVsTeamDescription;
            default:
                return "No Description Found";
        }
    }

    private string GetSubtitleText()
    {
        StringBuilder sb = new StringBuilder();
        int agents = 0;
        int player = 0;

        foreach(Team team in matchConfiguration.teamsConfig)
        {
            agents += team.agentsCount;
            if (team.isPlayer) player = 1;
        }

        sb.Append("Agents: ");
        sb.Append(agents);
        sb.Append("\n");

        if(player > 0)
        {
            sb.Append("Player: YES");
        }
        return sb.ToString();
    }

    private void SetText(TextMeshProUGUI textUI, String matchText)
    {
        textUI.text = matchText;
    }
}
