using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HealthPacks : Detected
{
    public HealthPacks(GameObject agent)
    {
        this.parent = agent;
    }
}
