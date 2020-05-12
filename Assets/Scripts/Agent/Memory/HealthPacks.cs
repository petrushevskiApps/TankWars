using UnityEngine;

public class HealthPacks : Detected
{
    public HealthPacks(GameObject agent)
    {
        this.parent = agent;
    }
}
