using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCNames", menuName = "Data/NPCNames", order = 1)]
public class NPCNames : ScriptableObject
{
    [SerializeField] List<string> npcNames = new List<string>();

    public string GetRandomName()
    {
        return npcNames[Random.Range(0, npcNames.Count)];
    }
}
