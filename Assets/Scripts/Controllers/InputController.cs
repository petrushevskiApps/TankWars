using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputController : MonoBehaviour
{
    public static UnityEvent OnBackKey = new UnityEvent();

    public static UnityEvent OnFirePressed = new UnityEvent();

    public static AxisEvent OnMovementAxis = new AxisEvent();
    
    public static AxisEvent OnTurningAxis = new AxisEvent();

    public static BoolEvent OnCollecting = new BoolEvent();

    public static UnityEvent OnBoostStart = new UnityEvent();
    public static UnityEvent OnBoostEnd = new UnityEvent();

    public static UnityEvent OnShieldToggle = new UnityEvent();

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnBackKey.Invoke();
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

        if(Input.GetKey(KeyCode.LeftShift))
        {
            OnBoostStart.Invoke();
        }

        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            OnBoostEnd.Invoke();
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            OnShieldToggle.Invoke();
        }
    }


    public class BoolEvent : UnityEvent<bool> { }
    public class AxisEvent : UnityEvent<float> { }
    
}
