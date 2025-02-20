﻿using GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AIDebuggerWindow : EditorWindow
{
    List<GameObject> selectedObjects;
    GUIStyle successText;
    GUIStyle failText;
    GUIStyle selectedText;

    [MenuItem("Debugger/AIDebugger")]
    private static void ShowWidnow()
    {
        GetWindow<AIDebuggerWindow>("AI Debugger");
    }

    private void OnGUI()
    {
        successText = new GUIStyle(EditorStyles.label);
        successText.normal.textColor = Color.blue;

        failText = new GUIStyle(EditorStyles.label);
        failText.normal.textColor = Color.red;

        selectedText = new GUIStyle(EditorStyles.label);
        selectedText.fontStyle = FontStyle.Bold;
        selectedText.normal.textColor = Color.magenta;

        selectedObjects = Selection.gameObjects.ToList();

        List<Team> teams = GameManager.Instance?.AgentsController.MatchTeams;

        if (teams != null && teams.Count > 0)
        {
            EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            foreach (Team team in teams)
            {
                PrintTeam(team.Members);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.LabelField("No Teams in Level Controller");
        }

    }
    Vector2 scrollPos;

    private void PrintTeam(List<Agent> team)
    {
        if (team.Count > 0)
        {
            foreach(AIAgent agent in team)
            {
                if(agent != null)
                {
                    GoapAgent goapAgent = agent.gameObject.GetComponent<GoapAgent>();

                    

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    if (goapAgent != null)
                    {
                        if(IsSelected(agent.gameObject))
                        {
                            GUILayout.Label("Agent Name: " + goapAgent.name, selectedText);
                        }
                        else
                        {
                            GUILayout.Label("Agent Name: " + goapAgent.name);
                        }

                        GUILayout.Label("    Plan: " + goapAgent.CurrentPlanTextual);
                        GUILayout.Label("    State: " + goapAgent.State.ToString());
                        GUILayout.Label("    Action: " + goapAgent.GetCurrentAction());
                        GUILayout.Label("    Health: " + agent.Inventory.Health.Amount.ToString());
                        GUILayout.Label("    Ammo: " + agent.Inventory.Ammo.Amount.ToString());
                        GUILayout.Label("    Agent Internal State: ");
                        PrintWorldState(agent.Memory?.GetWorldState());

                    }
                    EditorGUILayout.EndVertical();
                }
                
            }
            
        }
        else
        {
            EditorGUILayout.LabelField("No team members");
        }
        
    }
    public void PrintWorldState(Dictionary<string, bool> dict)
    {
        foreach (KeyValuePair<string, bool> kvp in dict)
        {
            if(kvp.Value)
            {
                GUILayout.BeginHorizontal(GUILayout.Width(70));
                EditorGUILayout.LabelField("	- " + kvp.Key + " = ");
                EditorGUILayout.LabelField(kvp.Value.ToString(), successText);
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal(GUILayout.Width(70));
                EditorGUILayout.LabelField("	- " + kvp.Key + " = ");
                EditorGUILayout.LabelField(kvp.Value.ToString(), failText);
                GUILayout.EndHorizontal();
            }
        }

    }

    private bool IsSelected(GameObject go)
    {
        foreach(GameObject selected in selectedObjects)
        {
            if(selected.Equals(go))
            {
                return true;
            }
        }
        return false;
    }
    public void OnInspectorUpdate()
    {
        // This will only get called 10 times per second.
        Repaint();
    }
}
