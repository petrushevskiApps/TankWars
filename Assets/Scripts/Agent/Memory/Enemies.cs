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
        this.agent = agent;
    }

}
