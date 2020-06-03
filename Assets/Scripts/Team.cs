using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Team : IEquatable<Team>
{
    public int TeamID { get; private set; }
    public string TeamName { get; private set; }
    public bool IsPlayerTeam { get; private set; }

    public int TeamMembersAlive => TeamMembers.Count;

    public List<Agent> TeamMembers { get; private set; }

    public TeamEvent TeamEmpty = new TeamEvent();

    public Team(int teamId, string teamName, bool isPlayerTeam, List<Agent> teamMembers)
    {
        TeamID = teamId;
        TeamName = teamName;
        IsPlayerTeam = isPlayerTeam;

        TeamMembers = teamMembers;
    }

    public void ActivateTeamMembers()
    {
        foreach (Agent agent in TeamMembers)
        {
            agent.GetComponent<IDestroyable>().RegisterOnDestroy(OnAgentDestroyed);
            agent.gameObject.SetActive(true);
            agent.gameObject.GetComponent<AudioListener>().enabled = false;
        }
    }
    private void OnAgentDestroyed(GameObject agent)
    {
        int agentIndex = TeamMembers.IndexOf(agent.GetComponent<Agent>());

        if (agentIndex >= 0)
        {
            TeamMembers.RemoveAt(agentIndex);
        }
        else
        {
            TeamMembers.RemoveAll(x => x == null);
        }

        if (TeamMembersAlive == 0) TeamEmpty.Invoke(this);
    }
    public bool Equals(Team other)
    {
        if (TeamID == other.TeamID) return true;
        else return false;
    }

    public class TeamEvent : UnityEvent<Team> {}
}
