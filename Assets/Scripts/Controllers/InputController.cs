using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputController : MonoBehaviour
{
    [HideInInspector]
    public static UnityEvent OnEscapePressed = new UnityEvent();

    [HideInInspector]
    public static UnityEvent OnFirePressed = new UnityEvent();

    [HideInInspector]
    public static AxisEvent OnMovementAxis = new AxisEvent();
    
    [HideInInspector]
    public static AxisEvent OnTurningAxis = new AxisEvent();

    [HideInInspector]
    public static CollectEvent OnCollecting = new CollectEvent();

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapePressed.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnFirePressed.Invoke();
        }
        
        if(Input.GetKeyDown(KeyCode.E))
        {
            OnCollecting.Invoke(true);
        }
        if(Input.GetKeyUp(KeyCode.E))
        {
            OnCollecting.Invoke(false);
        }

        OnMovementAxis.Invoke(Input.GetAxis("Vertical"));
        OnTurningAxis.Invoke(Input.GetAxis("Horizontal"));
    }


    public class CollectEvent : UnityEvent<bool> { }
    public class AxisEvent : UnityEvent<float> { }
    
}
