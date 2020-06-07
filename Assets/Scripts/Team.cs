using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Team : IEquatable<Team>, IComparable<Team>
{
    public int ID { get; private set; }
    public string TeamName { get; private set; }
    public bool IsPlayerTeam { get; private set; }

    public int TeamMembersAlive => Members.Count;
    public int TeamKills { get; private set; } = 0;

    public List<Agent> Members { get; private set; }

    public TeamEvent OnTeamEmpty = new TeamEvent();
    public TeamKillEvent OnTeamKill = new TeamKillEvent();

    

    public Team(int teamId, string teamName, bool isPlayerTeam, List<Agent> teamMembers)
    {
        ID = teamId;
        TeamName = teamName;
        IsPlayerTeam = isPlayerTeam;

        Members = teamMembers;
    }

    public void ActivateTeamMembers()
    {
        foreach (Agent agent in Members)
        {
            agent.GetComponent<IDestroyable>().RegisterOnDestroy(OnAgentDestroyed);
            agent.gameObject.SetActive(true);
            agent.gameObject.GetComponent<AudioListener>().enabled = false;
        }
    }

    public void IncreaseTeamKills()
    {
        TeamKills++;
        OnTeamKill.Invoke(TeamKills);
    }

    private void OnAgentDestroyed(GameObject agent)
    {
        int agentIndex = Members.IndexOf(agent.GetComponent<Agent>());

        if (agentIndex >= 0)
        {
            Members.RemoveAt(agentIndex);
        }
        else
        {
            Members.RemoveAll(x => x == null);
        }

        if (TeamMembersAlive == 0) OnTeamEmpty.Invoke(this);
    }
    public bool Equals(Team other)
    {
        if (ID == other.ID) return true;
        else return false;
    }

    public int CompareTo(Team other)
    {
        if (TeamKills > other.TeamKills) return -1;
        else if (TeamKills < other.TeamKills) return 1;
        else return 0;
    }

    public class TeamEvent : UnityEvent<Team> {}
    public class TeamKillEvent : UnityEvent<int> { }
}
