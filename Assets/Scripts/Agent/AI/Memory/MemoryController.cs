using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MemoryController : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent OnUnderFire = new UnityEvent();

    public DetectedHolder Enemies { get; private set; }

    public DetectedHolder AmmoPacks { get; private set; }
    
    public DetectedHolder HealthPacks { get; private set; }

    public DetectedHolder HidingSpots { get; private set; }


    public Vector3 MissileDirection { get; private set; }

    public bool IsUnderAttack { get; private set; }


    private Dictionary<string, Func<bool>> worldState = new Dictionary<string, Func<bool>>();

    private List<Dictionary<string, bool>> goals = new List<Dictionary<string, bool>>();

    private AIAgent agent;

    private Coroutine UnderAttackTimer;
    
    public void Initialize(AIAgent agent)
    {
        this.agent = agent;

        Enemies = new DetectedHolder(agent.gameObject, new EnemyComparator());
        AmmoPacks = new DetectedHolder(agent.gameObject, new DistanceComparator());
        HealthPacks = new DetectedHolder(agent.gameObject, new DistanceComparator());
        HidingSpots = new DetectedHolder(agent.gameObject, new DistanceComparator());

        SetStates();
        SetGoals();

        RegisterEvents();
    }
    private void OnDestroy()
    {
        UnregisterEvents();
    }

    private void SetStates()
    {
        worldState.Add(StateKeys.ENEMY_DETECTED, Enemies.IsAnyValidDetected);
        worldState.Add(StateKeys.UNDER_ATTACK, () => IsUnderAttack);

        worldState.Add(StateKeys.HEALTH_AMOUNT, IsHealthAvailable);
        worldState.Add(StateKeys.HEALTH_DETECTED, HealthPacks.IsAnyValidDetected);

        worldState.Add(StateKeys.AMMO_AVAILABLE, IsAmmoAvailable);
        worldState.Add(StateKeys.AMMO_DETECTED, AmmoPacks.IsAnyValidDetected);

        worldState.Add(StateKeys.HIDING_SPOT_DETECTED, HidingSpots.IsAnyValidDetected);

    }

    private void SetGoals()
    {
        // Survive
        goals.Add(new Dictionary<string, bool>() 
        {
            { StateKeys.ENEMY_DETECTED, false },
            { StateKeys.AMMO_AVAILABLE, true },
            { StateKeys.HEALTH_AMOUNT, true }, 
        });

        goals.Add(new Dictionary<string, bool>()
        {
            { StateKeys.PATROL, true },
        });
        
    }

    private void RegisterEvents()
    {
        agent.GetPerceptor().OnEnemyDetected.AddListener(Enemies.AddDetected);
        agent.GetPerceptor().OnEnemyLost.AddListener(Enemies.RemoveDetected);

        agent.GetPerceptor().OnHidingSpotDetected.AddListener(HidingSpots.AddDetected);
        agent.GetPerceptor().OnHidingSpotLost.AddListener(HidingSpots.RemoveDetected);

        agent.GetPerceptor().OnAmmoPackDetected.AddListener(AmmoPacks.AddDetected);
        agent.GetPerceptor().OnAmmoPackLost.AddListener(AmmoPacks.RevalidateDetected);

        agent.GetPerceptor().OnHealthPackDetected.AddListener(HealthPacks.AddDetected);
        agent.GetPerceptor().OnHealthPackLost.AddListener(HealthPacks.RevalidateDetected);

        agent.GetPerceptor().OnUnderAttack.AddListener(SetIsUnderAttack);

    }

    private void UnregisterEvents()
    {
        agent.GetPerceptor().OnEnemyDetected.RemoveListener(Enemies.AddDetected);
        agent.GetPerceptor().OnEnemyLost.RemoveListener(Enemies.RemoveDetected);

        agent.GetPerceptor().OnHidingSpotDetected.RemoveListener(HidingSpots.AddDetected);
        agent.GetPerceptor().OnHidingSpotLost.RemoveListener(HidingSpots.RemoveDetected);

        agent.GetPerceptor().OnAmmoPackDetected.RemoveListener(AmmoPacks.AddDetected);
        agent.GetPerceptor().OnAmmoPackLost.RemoveListener(AmmoPacks.RevalidateDetected);

        agent.GetPerceptor().OnHealthPackDetected.RemoveListener(HealthPacks.AddDetected);
        agent.GetPerceptor().OnHealthPackLost.RemoveListener(HealthPacks.RevalidateDetected);

        agent.GetPerceptor().OnUnderAttack.RemoveListener(SetIsUnderAttack);
    }


    public Dictionary<string, bool> GetWorldState()
    {
        Dictionary<string, bool> agentState = new Dictionary<string, bool>();

        foreach (KeyValuePair<string, Func<bool>> state in worldState)
        {
            agentState.Add(state.Key, state.Value());
        }

        return agentState;
    }

    public List<Dictionary<string, bool>> GetGoals()
    {
        return goals;
    }

    private void SetIsUnderAttack(GameObject missile)
    {
        if(!IsUnderAttack)
        {
            OnUnderFire.Invoke();
        }

        IsUnderAttack = true;
        MissileDirection = missile.gameObject.transform.position;

        if (UnderAttackTimer != null)
        {
            agent.StopCoroutine(UnderAttackTimer);
        }

        UnderAttackTimer = agent.StartCoroutine(UnderAttack());
    }

    IEnumerator UnderAttack()
    {
        yield return new WaitForSeconds(2f);
        UnderAttackTimer = null;
        IsUnderAttack = false;
        MissileDirection = Vector3.zero;
    }

    public bool IsAmmoAvailable()
    {
        InventoryStatus status = agent.Inventory.Ammo.Status;

        if (status == InventoryStatus.Empty)
        {
            return false;
        }
        else if (status == InventoryStatus.Low || status == InventoryStatus.Medium)
        {
            if (Enemies.IsAnyValidDetected())
            {
                return true;
            }
            else return false;
        }
        else
        {
            return true;
        }
    }
    public bool IsHealthAvailable()
    {
        InventoryStatus status = agent.Inventory.Health.Status;

        if (status == InventoryStatus.Low)
        {
            return false;
        }
        else if (status == InventoryStatus.Medium)
        {
            if (Enemies.IsAnyValidDetected())
            {
                return true;
            }
            else return false;
        }
        else
        {
            return true;
        }
    }
}
