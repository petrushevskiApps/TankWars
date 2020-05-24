using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemies : Detectable
{
    public Enemies(GameObject agent)
    {
        parent = agent;
    }

    public override Detected CreateDetected(GameObject detected, string detectedName, GameObject agent)
    {
        return new DetectedEnemy(detected, detectedName, agent);
    }
}
