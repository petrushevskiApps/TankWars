using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Sensor : MonoBehaviour
{
    public DetectedEvent OnDetected = new DetectedEvent();
    public LostEvent OnLost = new LostEvent();

    public class DetectedEvent : UnityEvent<GameObject, bool> { }
    public class LostEvent : UnityEvent<GameObject> { }
}
