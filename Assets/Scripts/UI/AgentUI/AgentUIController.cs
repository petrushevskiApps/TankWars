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
        SetName(agent.GetAgentName());
        SetHealthBar(agent.GetInventory());
        SetAmmoBar(agent.GetInventory());
    }
    private void SetName(string name)
    {
        agentNameText.text = name;
    }
    private void SetHealthBar(Inventory inventory)
    {
        healthBar.Initialize(inventory.GetHealth(), inventory.GetHealthCapacity(), inventory.OnHealthChange) ;
    }

    private void SetAmmoBar(Inventory inventory)
    {
        ammoBar.Initialize(inventory.GetAmmo(), inventory.GetAmmoCapacity(), inventory.OnAmmoChange);
    }

    
}
