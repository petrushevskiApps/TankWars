using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AmmoPacks : Detectable
{
    public AmmoPacks(GameObject agent)
    {
        parent = agent;
    }
    public override Detected CreateDetected(GameObject detected, string detectedName, GameObject agent)
    {
        return new Detected(detected, detectedName, agent);
    }

}
