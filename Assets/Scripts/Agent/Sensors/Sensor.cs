using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Sensor : MonoBehaviour
{
    public Detected OnDetected = new Detected();
    public Detected OnLost = new Detected();


    public class Detected : UnityEvent<GameObject, bool>
    {

    }
}
