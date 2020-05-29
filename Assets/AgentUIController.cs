using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentUIController : MonoBehaviour
{
    [SerializeField] private UIHealthBar healthBar;

    public void SetHealthBar(Inventory inventory)
    {
        healthBar.Initialize(inventory.GetHealth(), inventory.OnHealthChange);
    }
}
