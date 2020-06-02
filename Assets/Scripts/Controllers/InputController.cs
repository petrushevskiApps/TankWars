using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputController : MonoBehaviour
{
    [HideInInspector]
    public static UnityEvent OnEscapePressed = new UnityEvent();

    [HideInInspector]
    public static UnityEvent OnSpacePressed = new UnityEvent();

    [HideInInspector]
    public static AxisEvent OnMovementAxis = new AxisEvent();
    
    [HideInInspector]
    public static AxisEvent OnTurningAxis = new AxisEvent();

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapePressed.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnSpacePressed.Invoke();
        }

        OnMovementAxis.Invoke(Input.GetAxis("Vertical"));
        OnTurningAxis.Invoke(Input.GetAxis("Horizontal"));
    }

    public class AxisEvent : UnityEvent<float>
    {

    }
}
