using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectedEnemy : Detected
{
    public float healthDifferenceThreshold = 2f;

    public DetectedEnemy(GameObject detected, string detectedName, GameObject agent)
        : base(detected, detectedName, agent) { }


    // If there is noticable difference ( threshold )
    // in enemies health, compare by Health, otherwise
    // compare by distance from the agent.
    public override int CompareTo(Detected other)
    {
        if (IsValid() && other.IsValid())
        {
            float otherHealth = other.agent.GetComponent<Agent>().GetInventory().GetHealth();
            float health = agent.GetComponent<Agent>().GetInventory().GetHealth();
            
            if(Mathf.Abs(health - otherHealth) > healthDifferenceThreshold)
            {
                return health < otherHealth ? -1 : 1;
            }
            else
            {
                float distance = GetDistance();
                float otherDistance = other.GetDistance();

                if (distance < otherDistance)
                {
                    return -1;
                }
                else if (distance > otherDistance)
                {
                    return 1;
                }
                else return 0;
            }
            
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
}
