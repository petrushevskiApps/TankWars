using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarSensor : Sensor
{
    private void OnTriggerStay(Collider other)
    {
        OnDetected.Invoke(other.gameObject, true);
    }

    private void OnTriggerExit(Collider other)
    {
        OnLost.Invoke(other.gameObject);
    }
}
