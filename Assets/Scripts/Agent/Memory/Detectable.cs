using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class Detectable
{
    protected GameObject parent;

    public List<Detected> detectables = new List<Detected>();

    public bool IsAnyValidDetected()
    {
        int count = 0;

        if(detectables.Count == 0)
        {
            return false;
        }

        foreach(Detected detectable in detectables)
        {
            if (detectable.IsValid())
            {
                count++;
            }
        }

        return count > 0;
    }
    
    public void AddDetected(GameObject detected)
    {
        Detected detectable = new Detected(detected, detected.name, parent);

        if(!detectables.Contains(detectable))
        {
            detected.GetComponent<IDestroyable>()?.RegisterOnDestroy(RemoveDetected);
            detectables.Add(detectable);
            Debug.Log($"<color=green>InternalState::{this.GetType()} Added</color>");
        }
    }

    public void InvalidateDetected(GameObject detected)
    {
        Detected detectable = new Detected(detected, detected.name, parent);

        if(detectables.Contains(detectable))
        {
            detectables[detectables.IndexOf(detectable)].status = false;
        }
    }
    public void ValidateDetected(GameObject detected)
    {
        Detected detectable = new Detected(detected, detected.name, parent);

        if (detectables.Contains(detectable))
        {
            Detected detectableRef = detectables[detectables.IndexOf(detectable)];
            parent.GetComponent<Agent>().StartCoroutine(RevalidationTimer(detectableRef));
        }
    }
    IEnumerator RevalidationTimer(Detected detectable)
    {
        yield return new WaitForSeconds(5f);
        detectable.status = true;
    }
    public void RemoveDetected(GameObject detected) 
    {
        if(detected != null)
        {
            Detected detectable = new Detected(detected, detected.name, parent);
            detectables.Remove(detectable);
        }
        else
        {
            List<Detected> toRemove = new List<Detected>();

            foreach (Detected detectable in detectables)
            {
                if (detectable.detected == null)
                {
                    toRemove.Add(detectable);
                }
            }

            foreach (Detected detectable in toRemove)
            {
                detectables.Remove(detectable);
            }
        }
        
    }

    // Return closes detected with valid status
    public GameObject GetDetected()
    {
        if(IsAnyValidDetected())
        {
            detectables.Sort();

            Detected detected = detectables[0];

            if(detected.detected != null)
            {
                return detected.detected;
            }
        }
        return null;
    }

    public bool IsDetectedValid(GameObject detected)
    {
        Detected detectable = new Detected(detected, detected.name, parent);

        if (detectables.Contains(detectable))
        {
            return detectable.IsValid();
        }
        return false;
    }
    
    
}


