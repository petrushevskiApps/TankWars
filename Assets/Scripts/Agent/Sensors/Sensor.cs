using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Sensor : MonoBehaviour
{
    public SensorEvent OnVisibleDetected = new SensorEvent();
    public SensorEvent OnDetected = new SensorEvent();
    public SensorEvent OnLost = new SensorEvent();

    public class SensorEvent : UnityEvent<GameObject, bool>
    {

    }
}
