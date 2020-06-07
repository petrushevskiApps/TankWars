using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AgentUIController : MonoBehaviour
{
    [SerializeField] private UISlider healthBar;
    [SerializeField] private UISlider ammoBar;
    [SerializeField] private TextMeshProUGUI agentNameText;

    public void Setup(Agent agent)
    {
        SetName(agent.AgentName);
        SetHealthBar(agent.Inventory);
        SetAmmoBar(agent.Inventory);
    }
    private void SetName(string name)
    {
        agentNameText.text = name;
    }
    private void SetHealthBar(InventorySystem inventory)
    {
        healthBar.Initialize(inventory.Health.Amount, inventory.Health.Capacity, inventory.Health.AmountChanged) ;
    }

    private void SetAmmoBar(InventorySystem inventory)
    {
        ammoBar.Initialize(inventory.Ammo.Amount, inventory.Ammo.Capacity, inventory.Ammo.AmountChanged);
    }

    
}
