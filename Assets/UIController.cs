using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private UIHealthBar healthBar;

    public void SetHealthBar(Inventory inventory)
    {
        healthBar.Initialize(inventory.GetHealth(), inventory.OnHealthChange);
    }
}
