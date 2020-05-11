using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class Enemies : Detected
{

    public Enemies(GameObject agent)
    {
        this.parent = agent;
    }

    public bool InShootingRange(float range)
    {
        if(IsAnyValidDetected())
        {
            GameObject enemy = GetDetected();
            return Vector3.Distance(parent.transform.position, enemy.transform.position) >= range;
        }
        return false;
    }
}
