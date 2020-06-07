using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DetectedHolder
{
    public UnityEvent OnDetected = new UnityEvent();
    public UnityEvent OnRemoved = new UnityEvent();

    protected GameObject agent;
    private IComparator comparator;

    public List<Detected> detectables = new List<Detected>();

    public DetectedHolder(GameObject agent, IComparator comparator)
    {
        this.agent = agent;
        this.comparator = comparator;
    }
 
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
        Detected detectable = new Detected(detected, detected.name, agent, comparator);

        if (!detectables.Contains(detectable))
        {
            // Notify all listeners about detected
            OnDetected.Invoke();
            // Listen when detected is destroyed / collected
            detected.GetComponent<IDestroyable>()?.RegisterOnDestroy(RemoveDetected);
            // Add detected to list of detectables
            detectables.Add(detectable);
        }
    }

    public void RemoveDetected(GameObject detectedObject)
    {
        if (detectedObject != null)
        {
            Detected detected = GetDetected(detectedObject);
            detectables.Remove(detected);
            OnRemoved.Invoke();
        }
        else
        {
            detectables.RemoveAll(detectable => detectable.detected == null);
        }

    }

    public void InvalidateDetected(GameObject detectedObject)
    {
        Detected detected = GetDetected(detectedObject);

        if(detected != null)
        {
            detected.status = false;
        }
    }

    public void RevalidateDetected(GameObject detectedObject)
    {
        Detected detected = GetDetected(detectedObject);

        if (detected != null)
        {
            agent.GetComponent<Agent>().StartCoroutine(RevalidationTimer(detected));
        }
    }

    IEnumerator RevalidationTimer(Detected detectable)
    {
        yield return new WaitForSeconds(5f);
        detectable.status = true;
    }

    

    // Return closes detected with valid status
    public GameObject GetSortedDetected()
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
        return detectables.FindAll(detectable => detectable.IsValid()).Count;
    }

    public bool IsDetectedValid(GameObject detected)
    {
        if(detected != null)
        {
            return GetDetected(detected)?.IsValid() ?? false;
        }
        
        return false;
    }
    
    private Detected GetDetected(GameObject detectedObject)
    {
        Detected detected = detectables.FirstOrDefault(detectable => detectable.detected.Equals(detectedObject));
        return detected;
    }
    
}


