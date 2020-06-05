using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This Scriptable object contains list of names
 * to be assigned to agents on creation.
 */
[CreateAssetMenu(fileName = "Names", menuName = "Data/Names", order = 1)]
public class Names : ScriptableObject
{
    public List<string> names = new List<string>();

    
}
