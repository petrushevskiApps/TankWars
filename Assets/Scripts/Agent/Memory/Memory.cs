using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MemorySystem
{
    
    public float shootingRange = 10;

    public Enemies Enemies { get; private set; }

    public AmmoPacks AmmoPacks { get; private set; }
    
    public HealthPacks HealthPacks { get; private set; }

    public HidingSpots HidingSpots { get; private set; }

    private Dictionary<string, Func<bool>> worldState = new Dictionary<string, Func<bool>>();

    private List<Dictionary<string, bool>> goals = new List<Dictionary<string, bool>>();

    private Agent parent;

    public bool IsUnderAttack { get; private set; }

    public void Initialize(Agent parent)
    {
        this.parent = parent;

        Enemies = new Enemies(parent.gameObject);
        AmmoPacks = new AmmoPacks(parent.gameObject);
        HealthPacks = new HealthPacks(parent.gameObject);
        HidingSpots = new HidingSpots(parent.gameObject);

        SetStates();
        SetGoals();
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

    public void RegisterEvents(PerceptorSystem perceptor)
    {
        perceptor.OnEnemyDetected.AddListener(Enemies.AddDetected);
        perceptor.OnEnemyLost.AddListener(Enemies.RemoveDetected);
        
        perceptor.OnAmmoPackDetected.AddListener(AmmoPacks.AddDetected);
        perceptor.OnAmmoPackLost.AddListener(AmmoPacks.ValidateDetected);

        perceptor.OnHealthPackDetected.AddListener(HealthPacks.AddDetected);
        perceptor.OnHealthPackLost.AddListener(HealthPacks.ValidateDetected);
        
        perceptor.OnHidingSpotDetected.AddListener(HidingSpots.AddDetected);
        perceptor.OnHidingSpotLost.AddListener(HidingSpots.RemoveDetected);

        perceptor.OnUnderAttack.AddListener(SetIsUnderAttack);

    }

    public void RemoveEvents(PerceptorSystem perceptor)
    {
        perceptor.OnEnemyDetected.RemoveListener(Enemies.AddDetected);
        perceptor.OnEnemyLost.RemoveListener(Enemies.RemoveDetected);
        
        perceptor.OnAmmoPackDetected.RemoveListener(AmmoPacks.AddDetected);
        perceptor.OnAmmoPackLost.RemoveListener(AmmoPacks.RemoveDetected);
        
        perceptor.OnHealthPackDetected.RemoveListener(HealthPacks.AddDetected);
        perceptor.OnHealthPackLost.RemoveListener(HealthPacks.RemoveDetected);
        
        perceptor.OnHidingSpotDetected.RemoveListener(HidingSpots.AddDetected);
        perceptor.OnHidingSpotLost.RemoveListener(HidingSpots.RemoveDetected);

        perceptor.OnUnderAttack.RemoveListener(SetIsUnderAttack);
    }
    private Coroutine UnderAttackTimer;

    private void SetIsUnderAttack(GameObject enemy)
    {
        IsUnderAttack = true;

        if (UnderAttackTimer != null)
        {
            parent.StopCoroutine(UnderAttackTimer);
        }

        UnderAttackTimer = parent.StartCoroutine(UnderAttack());
    }

    IEnumerator UnderAttack()
    {
        yield return new WaitForSeconds(5f);
        UnderAttackTimer = null;
        IsUnderAttack = false;
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


    public bool IsAmmoAvailable()
    {
        InventoryStatus status = parent.GetInventory().AmmoStatus;

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
        InventoryStatus status = parent.GetInventory().HealthStatus;

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
