using GOAP;
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

        List<List<Agent>> teams = LevelController.GetTeamsList();

        if (teams != null && teams.Count > 0)
        {
            EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            foreach (List<Agent> team in teams)
            {
                PrintTeam(team);
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
            
            foreach(AI tank in team)
            {
                if(tank != null)
                {
                    GameObject agentGO = tank.gameObject;
                    GoapAgent agent = agentGO.GetComponent<GoapAgent>();

                    

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    if (agent != null)
                    {
                        if(IsSelected(agentGO))
                        {
                            GUILayout.Label("Agent Name: " + agent.name, selectedText);
                        }
                        else
                        {
                            GUILayout.Label("Agent Name: " + agent.name);
                        }

                        GUILayout.Label("    Plan: " + agent.GetCurrentPlanString());
                        GUILayout.Label("    State: " + agent.State.ToString());
                        GUILayout.Label("    Action: " + agent.GetCurrentAction());
                        GUILayout.Label("    Health: " + tank.GetInventory().GetHealth().ToString());
                        GUILayout.Label("    Ammo: " + tank.GetInventory().GetAmmo().ToString());
                        GUILayout.Label("    Agent Internal State: ");
                        PrintWorldState(tank.GetMemory().GetWorldState());

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
