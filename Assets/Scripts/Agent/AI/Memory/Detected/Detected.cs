using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detected : IEquatable<Detected>, IComparable<Detected>
{
    public GameObject agent;
    public GameObject detected;
    public string detectedName;
    public bool status;

    public Detected(GameObject detected, string detectedName, GameObject agent)
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
    public virtual int CompareTo(Detected other)
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

    public bool Equals(Detected other)
    {
        if (detectedName.Equals(other.detectedName))
        {
            return true;
        }
        else return false;
    }
}
