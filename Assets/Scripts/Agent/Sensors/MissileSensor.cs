using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSensor : Sensor
{
    private void OnTriggerEnter(Collider other)
    {
        OnDetected.Invoke(other.gameObject);
    }
}
