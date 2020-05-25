using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioSensor : Sensor
{
    public void ReceiveLocations(List<GameObject> detectedList)
    {
        foreach (GameObject detected in detectedList)
        {
            OnVisibleDetected.Invoke(detected, true);
        }
    }

    public void CallForHelp(GameObject enemy)
    {
        OnVisibleDetected.Invoke(enemy, false);
    }

}
