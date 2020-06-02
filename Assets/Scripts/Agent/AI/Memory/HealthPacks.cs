using System.Collections.Generic;
using UnityEngine;

public class HealthPacks : Detectable
{
    public HealthPacks(GameObject agent)
    {
        parent = agent;
    }

    public override Detected CreateDetected(GameObject detected, string detectedName, GameObject agent)
    {
        return new Detected(detected, detectedName, agent);
    }
}
