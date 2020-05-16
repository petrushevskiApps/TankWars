using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Sensor : MonoBehaviour
{
    public Detected OnVisibleDetected = new Detected();
    public Detected OnInisibleDetected = new Detected();
    public Detected OnLost = new Detected();


    public class Detected : UnityEvent<GameObject>
    {

    }
}
