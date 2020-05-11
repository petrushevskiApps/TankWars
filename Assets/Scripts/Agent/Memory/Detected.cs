using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public abstract class Detected
{
    protected GameObject parent;

    [SerializeField] public List<Detectable> detectedList = new List<Detectable>();

    public bool IsAnyValidDetected()
    {
        int count = 0;
        if(detectedList.Count == 0)
        {
            return false;
        }

        foreach(Detectable detectable in detectedList)
        {
            if (detectable.status)
            {
                if(detectable.detected != null)
                {
                    count++;
                }
                else
                {
                    detectedList.Remove(detectable);
                    return IsAnyValidDetected();
                }
            }
        }

        return count > 0;
    }

    public void InvalidateDetected(GameObject ammoPack)
    {
        Detectable detectable = new Detectable(ammoPack, true, parent);

        if(detectedList.Contains(detectable))
        {
            detectedList[detectedList.IndexOf(detectable)].status = false;
            detectedList.Sort();
        }
    }

    public void AddDetected(GameObject detected)
    {
        Detectable detectable = new Detectable(detected, true, parent);

        if(!detectedList.Contains(detectable))
        {
            detectedList.Add(detectable);
            detectedList.Sort();
            Debug.Log($"<color=green>InternalState::{this.GetType()} Added</color>");
        }
    }

    public void RemoveDetected(GameObject detected) 
    {
        Detectable detectable = new Detectable(detected, true, parent);

        if (detectedList.Contains(detectable))
        {
            detectedList.Remove(detectable);
        }
    }

    // Return closes detected with valid status
    public GameObject GetDetected()
    {

        if(IsAnyValidDetected())
        {
            Detectable detected = detectedList[0];

            if(detected.detected != null && detected.status)
            {
                return detected.detected;
            }
            else
            {
                detectedList.Remove(detected);
                detectedList.Sort();
                
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
        public bool status;

        public Detectable(GameObject detected, bool status, GameObject agent)
        {
            this.detected = detected;
            this.status = status;
            this.agent = agent;
        }

        public int CompareTo(Detectable other)
        {
            if (status && !other.status) return 1;
            else if (!status && other.status) return -1;
            else
            {
                if(detected != null && other.detected != null)
                {
                    float distanceX = Vector3.Distance(agent.transform.position, detected.transform.position);
                    float distanceY = Vector3.Distance(agent.transform.position, other.detected.transform.position);

                    if (distanceX > distanceY) return 1;
                    else if (distanceX < distanceY) return -1;
                    else return 0;
                }
                else
                {
                    if(detected == null)
                    {
                        status = false;
                        return -1;
                    }
                    if (other.detected == null)
                    {
                        other.status = false;
                        return 1;
                    }

                    return 0;
                }
            }
            
        }

        public bool Equals(Detectable other)
        {
            if (detected.Equals(other.detected)) return true;
            else return false;
        }
    }
}


