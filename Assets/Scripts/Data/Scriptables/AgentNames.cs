using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This Scriptable object contains list of names
 * to be assigned to agents on creation.
 */
[CreateAssetMenu(fileName = "AgentNames", menuName = "Data/AgentNames", order = 1)]
public class AgentNames : ScriptableObject
{
    [SerializeField] List<string> agentNames = new List<string>();

    private static List<string> availableNames;

    public void Setup()
    {
        availableNames = new List<string>();

        if (availableNames.Count <= 0)
        {
            agentNames.ForEach(name => availableNames.Add(name));
        }
    }

    public string GetRandomName()
    {
        if(availableNames == null)
        {
            Setup();
        }

        if(availableNames.Count > 0)
        {
            int index = Random.Range(0, availableNames.Count);
            string name = availableNames[index];
            availableNames.RemoveAt(index);
            return name;

        }

        return "NoNameAvailable";
    }
}
