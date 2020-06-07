using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollector
{
    void PickableCollected(AmmoPack collected);
    void PickableCollected(HealthPack collected);
}
