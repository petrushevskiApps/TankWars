using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class Detected
{
    protected GameObject parent;

    [SerializeField] public List<Detectable> detectables = new List<Detectable>();

    public bool IsAnyValidDetected()
    {
        int count = 0;

        if(detectables.Count == 0)
        {
            return false;
        }

        foreach(Detectable detectable in detectables)
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
        Detectable detectable = new Detectable(detected, detected.name, parent);

        if(!detectables.Contains(detectable))
        {
            detected.GetComponent<IDestroyable>()?.RegisterOnDestroy(RemoveDetected);
            detectables.Add(detectable);
            Debug.Log($"<color=green>InternalState::{this.GetType()} Added</color>");
        }
    }

    public void InvalidateDetected(GameObject detected)
    {
        Detectable detectable = new Detectable(detected, detected.name, parent);

        if(detectables.Contains(detectable))
        {
            detectables[detectables.IndexOf(detectable)].status = false;
        }
    }
    public void ValidateDetected(GameObject detected)
    {
        Detectable detectable = new Detectable(detected, detected.name, parent);

        if (detectables.Contains(detectable))
        {
            Detectable detectableRef = detectables[detectables.IndexOf(detectable)];
            parent.GetComponent<Agent>().StartCoroutine(RevalidationTimer(detectableRef));
        }
    }
    IEnumerator RevalidationTimer(Detectable detectable)
    {
        yield return new WaitForSeconds(5f);
        detectable.status = true;
    }
    public void RemoveDetected(GameObject detected) 
    {
        if(detected != null)
        {
            Detectable detectable = new Detectable(detected, detected.name, parent);
            detectables.Remove(detectable);
        }
        else
        {
            List<Detectable> toRemove = new List<Detectable>();

            foreach (Detectable detectable in detectables)
            {
                if (detectable.detected == null)
                {
                    toRemove.Add(detectable);
                }
            }

            foreach (Detectable detectable in toRemove)
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

            Detectable detected = detectables[0];

            if(detected.detected != null)
            {
                return detected.detected;
            }
        }
        return null;
    }

    
    public class Detectable : IEquatable<Detectable>, IComparable<Detectable>
    {
        public GameObject agent;
        public GameObject detected;
        public string detectedName;
        public bool status;

        public Detectable(GameObject detected,string detectedName, GameObject agent)
        {
            this.detected = detected;
            this.agent = agent;
            this.detectedName = detectedName;
            status = true;
        }

        public bool IsValid()
        {
            return detected != null && detected.activeSelf && status;
        }

        public float GetDistance()
        {
            return Vector3.Distance(agent.transform.position, detected.transform.position);
        }
        public int CompareTo(Detectable other)
        {
            
            if (IsValid() && other.IsValid())
            {
                if (GetDistance() > other.GetDistance()) return 1;
                else if (GetDistance() < other.GetDistance()) return -1;
                else return 0;
            }
            else if (IsValid() && !other.IsValid())
            {
                return -1;
            }
            else if (!IsValid() && other.IsValid())
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public bool Equals(Detectable other)
        {
            if (detectedName.Equals(other.detectedName))
            {
                return true;
            }
            else return false;
        }
    }
}


