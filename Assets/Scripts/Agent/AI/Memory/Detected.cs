using System;
using UnityEngine;

public class Detected : IEquatable<Detected>, IComparable<Detected>
{
    private GameObject agent;
    public GameObject detected;
    public string detectedName;
    public bool status;

    private IComparator comparator;

    public Detected(GameObject detected, string detectedName, GameObject agent, IComparator comparator)
    {
        this.detected = detected;
        this.agent = agent;
        this.detectedName = detectedName;
        this.comparator = comparator;
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

    public Agent GetAgent()
    {
        return agent.GetComponent<Agent>();
    }

    public int CompareTo(Detected other)
    {
        return comparator.CompareDetected(this, other);
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
