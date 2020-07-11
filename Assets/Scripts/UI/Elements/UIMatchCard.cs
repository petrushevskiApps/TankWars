using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMatchCard : MonoBehaviour
{
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
        GameManager.Instance.SetupMatch(matchConfiguration);
    }

    public void Setup(MatchConfiguration matchConfiguration)
    {
        this.matchConfiguration = matchConfiguration;
        SetText(subtitle, GetSubtitleText());
        SetText(description, GetDescriptionText());
        gameObject.SetActive(true);
    }

    private string GetDescriptionText()
    {
        return Constants.TeamVsTeamDescription;
    }

    private string GetSubtitleText()
    {
        StringBuilder sb = new StringBuilder();
        int agents = 0;
        int player = 0;

        sb.Append("Teams: " + matchConfiguration.teamsConfig.Count + "\n");
        
        foreach(TeamData team in matchConfiguration.teamsConfig)
        {
            agents += team.agentsCount;
            if (team.isPlayerTeam) player = 1;
        }

        sb.Append("Agents: ");
        sb.Append(agents);
        sb.Append("\n");
        sb.Append("Player: ");

        if (player > 0)
        {
            sb.Append("YES");
        }
        else sb.Append("NO");

        return sb.ToString();
    }

    private void SetText(TextMeshProUGUI textUI, String matchText)
    {
        textUI.text = matchText;
    }
}
