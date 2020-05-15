using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class Detected
{
    public UnityEvent OnDetected = new UnityEvent();
    public UnityEvent OnLost = new UnityEvent();

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
            else
            {
                detectables.Remove(detectable);
                return IsAnyValidDetected();
            }
        }

        return count > 0;
    }
    
    public void AddDetected(GameObject detected)
    {
        Detectable detectable = new Detectable(detected, parent);

        if(!detectables.Contains(detectable))
        {
            detectables.Add(detectable);
            detectables.Sort();
            OnDetected.Invoke();
            Debug.Log($"<color=green>InternalState::{this.GetType()} Added</color>");
        }
    }

    public void RemoveDetected(GameObject detected) 
    {
        Detectable detectable = new Detectable(detected, parent);

        if (detectables.Contains(detectable))
        {
            detectables.Remove(detectable);
            OnLost.Invoke();
        }
    }

    // Return closes detected with valid status
    public GameObject GetDetected()
    {
        if(IsAnyValidDetected())
        {
            Detectable detected = detectables[0];

            if(detected.detected != null)
            {
                return detected.detected;
            }
            else
            {
                detectables.Remove(detected);
                detectables.Sort();
                
                if(IsAnyValidDetected())
                {
                    return GetDetected();
                }
            }
        }
        return null;
    }

    public class Detectable : IEquatable<Detectable>, IComparable<Detectable>
    {
        public GameObject agent;
        public GameObject detected;

        public Detectable(GameObject detected, GameObject agent)
        {
            this.detected = detected;
            this.agent = agent;
        }

        public bool IsValid()
        {
            return detected != null && detected.activeSelf;
        }

        public int CompareTo(Detectable other)
        {
            if (detected != null && other.detected != null)
            {
                float distanceX = Vector3.Distance(agent.transform.position, detected.transform.position);
                float distanceY = Vector3.Distance(agent.transform.position, other.detected.transform.position);

                if (distanceX > distanceY) return 1;
                else if (distanceX < distanceY) return -1;
                else return 0;
            }
            else
            {
                if (detected == null)
                {
                    return -1;
                }
                if (other.detected == null)
                {
                    return 1;
                }

                return 0;
            }
        }

        public bool Equals(Detectable other)
        {
            if (detected.Equals(other.detected)) return true;
            else return false;
        }
    }
}


