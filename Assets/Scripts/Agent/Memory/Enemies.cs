using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Enemies : Detected
{
    private GameObject agent;

    public Enemies(GameObject agent)
    {
        this.agent = agent;
    }

    protected override int CompareDetected(GameObject x, GameObject y)
    {
        float distanceX = Vector3.Distance(agent.transform.position, x.transform.position);
        float distanceY = Vector3.Distance(agent.transform.position, y.transform.position);

        if (distanceX > distanceY) return 1;
        else if (distanceX < distanceY) return -1;
        else return 0;
    }
}
