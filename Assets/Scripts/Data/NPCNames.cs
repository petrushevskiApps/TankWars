using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCNames", menuName = "Data/NPCNames", order = 1)]
public class NPCNames : ScriptableObject
{
    [SerializeField] List<string> npcNames = new List<string>();

    private static List<string> availableNames;

    public void Setup()
    {
        availableNames = new List<string>();

        if (availableNames.Count <= 0)
        {
            foreach (string npcName in npcNames)
            {
                availableNames.Add(npcName);
            }
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
            string npcName = availableNames[index];
            availableNames.RemoveAt(index);
            return npcName;

        }

        return "NoNameAvailable";
    }
}
