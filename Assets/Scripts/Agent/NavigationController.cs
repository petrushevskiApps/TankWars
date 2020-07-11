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

    public float currentSpeed = 3.5f;
    private float originalSpeed = 3.5f;                // How fast the tank moves forward and back.
    private float maxSpeed = 8f;               // Maxiumum speed that can be reached with boost

    private MoveStatus AgentMoveStatus;
    private Coroutine Boosting;

    protected void Awake()
    {
        currentSpeed = originalSpeed;
    }


    public void StartBoosting(Agent agent)
    {
        Boosting = StartCoroutine(BoostSpeed(agent));
        agent.Inventory.SpeedBoost.Use();
    }
    public void StopBoosting(Agent agent)
    {
        if(Boosting != null)
        {
            StopCoroutine(Boosting);
            Boosting = null;
        }
        DecreaseSpeed();
        agent.Inventory.SpeedBoost.Refill();
    }

    // Increase speed gradually while there is boost
    // available in inventory.
    public IEnumerator BoostSpeed(Agent agent)
    {
        while(agent.Inventory.SpeedBoost.Amount > 0)
        {
            IncreaseSpeed();
            yield return new WaitForEndOfFrame();
        }
        DecreaseSpeed();
    }

    protected virtual void IncreaseSpeed()
    {
        currentSpeed = Mathf.Clamp(currentSpeed + Time.deltaTime, originalSpeed, maxSpeed);
    }
    protected virtual void DecreaseSpeed()
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
