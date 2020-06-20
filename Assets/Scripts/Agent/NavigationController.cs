using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NavigationController : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent OnAgentMoving = new UnityEvent();

    [HideInInspector]
    public UnityEvent OnAgentIdling = new UnityEvent();

    public float originalSpeed = 3.5f;                // How fast the tank moves forward and back.
    public float currentSpeed = 3.5f;
    public float turnSpeed = 130f;            // How fast the tank turns in degrees per second.
    public float maxSpeed = 5f;               // Maxiumum speed that can be reached with boost

    private MoveStatus AgentMoveStatus;

    protected void Awake()
    {
        currentSpeed = originalSpeed;
    }

    public virtual void BoostSpeed()
    {
        currentSpeed = Mathf.Clamp(currentSpeed + 0.01f, originalSpeed, maxSpeed);
    }
    public virtual void ResetSpeed()
    {
        currentSpeed = originalSpeed;
    }

    protected void OnMovement(bool isMoving)
    {
        if (!isMoving)
        {
            if (AgentMoveStatus != MoveStatus.Idling)
            {
                AgentMoveStatus = MoveStatus.Idling;
                OnAgentIdling.Invoke();
            }
        }
        else
        {
            if (AgentMoveStatus != MoveStatus.Moving)
            {
                AgentMoveStatus = MoveStatus.Moving;
                OnAgentMoving.Invoke();
            }
        }

    }

    public enum MoveStatus
    {
        None,
        Idling,
        Moving
    }
}
