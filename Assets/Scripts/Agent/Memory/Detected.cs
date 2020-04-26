using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Detected
{
    public Dictionary<string, GameObject> detectedObjects = new Dictionary<string, GameObject>();
    
    public List<GameObject> detectedList = new List<GameObject>();

    public bool IsAnyDetected()
    {
        return detectedList.Count > 0;
    }

    protected abstract int CompareDetected(GameObject x, GameObject y);

    
    public void AddDetected(GameObject detected)
    {
        if(!detectedList.Contains(detected))
        {
            detectedList.Add(detected);
            detectedList.Sort(CompareDetected);
            Debug.Log($"<color=green>InternalState::{this.GetType()} Added</color>");
        }
    }

    public void RemoveDetected(GameObject detected) 
    {
        if (!detectedList.Contains(detected))
        {
            detectedList.Remove(detected);
            Debug.Log($"<color=red>InternalState::{this.GetType()} Removed | Count: " + detectedObjects.Count + "</color>");
        }
    }


    public GameObject GetDetected()
    {
        if(IsAnyDetected())
        {
            GameObject detected = detectedList[0];

            if(detected != null)
            {
                return detected;
            }
            else
            {
                detectedList.Remove(detected);
                detectedList.Sort(CompareDetected);
                
                if(IsAnyDetected())
                {
                    return GetDetected();
                }
            }
        }
        return null;
    }
}
