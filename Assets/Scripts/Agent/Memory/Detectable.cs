using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class Detectable
{
    public UnityEvent OnDetected = new UnityEvent();
    public UnityEvent OnRemoved = new UnityEvent();

    protected GameObject parent;

    public List<Detected> detectables = new List<Detected>();

    public abstract Detected CreateDetected(GameObject detected, string detectedName, GameObject agent);

    public List<GameObject> GetDetectedList()
    {
        List<GameObject> detectedList = new List<GameObject>();

        foreach(Detected detected in detectables)
        {
            if(detected.IsValid())
            {
                detectedList.Add(detected.detected);
            }
        }
        return detectedList;
    }

    public void AddDetected(GameObject detected)
    {
        Detected detectable = CreateDetected(detected, detected.name, parent);

        if(!detectables.Contains(detectable))
        {
            OnDetected.Invoke();
            detected.GetComponent<IDestroyable>()?.RegisterOnDestroy(RemoveDetected);
            detectables.Add(detectable);
            Debug.Log($"<color=green>InternalState::{this.GetType()} Added</color>");
        }
    }

    public void RemoveDetected(GameObject detected)
    {
        if (detected != null)
        {
            OnRemoved.Invoke();
            Detected detectable = CreateDetected(detected, detected.name, parent);
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

    public void InvalidateDetected(GameObject detected)
    {
        Detected detectable = CreateDetected(detected, detected.name, parent);

        if(detectables.Contains(detectable))
        {
            detectables[detectables.IndexOf(detectable)].status = false;
        }
    }
    public void ValidateDetected(GameObject detected)
    {
        Detected detectable = CreateDetected(detected, detected.name, parent);

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

    public bool IsAnyValidDetected()
    {
        if (detectables.Count == 0)
        {
            return false;
        }

        return GetValidDetectedCount() > 0;
    }

    public int GetValidDetectedCount()
    {
        int count = 0;

        foreach (Detected detectable in detectables)
        {
            if (detectable.IsValid())
            {
                count++;
            }
        }

        return count;
    }
    public bool IsDetectedValid(GameObject detected)
    {
        if(detected != null)
        {
            Detected detectable = CreateDetected(detected, detected.name, parent);

            if (detectables.Contains(detectable))
            {
                return detectable.IsValid();
            }
        }
        
        return false;
    }
    
    
}


