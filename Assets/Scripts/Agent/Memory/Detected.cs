using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Detected
{
    public Dictionary<string, GameObject> detectedObjects = new Dictionary<string, GameObject>();
    
    public bool IsAnyDetected()
    {
        return detectedObjects.Count > 0;
    }

    public void AddDetected(GameObject detected)
    {
        if (!detectedObjects.ContainsKey(detected.name))
        {
            detectedObjects.Add(detected.name, detected);
            Debug.Log($"<color=green>InternalState::{this.GetType()} Added</color>");
        }
    } 
    public void RemoveDetected(string enemyKey)
    {
        if (detectedObjects.ContainsKey(enemyKey))
        {
            detectedObjects.Remove(enemyKey);
            Debug.Log($"<color=red>InternalState::{this.GetType()} Removed | Count: " + detectedObjects.Count + "</color>");
        }
    }
    public GameObject GetDetected()
    {
        if (IsAnyDetected())
        {
            KeyValuePair<string, GameObject> enemy = detectedObjects.FirstOrDefault();

            if (enemy.Value != null)
            {
                return enemy.Value;
            }
            else
            {
                detectedObjects.Remove(enemy.Key);
            }
        }
        return null;
    }
}
