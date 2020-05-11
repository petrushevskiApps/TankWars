using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;                              // The slider to represent how much health the tank currently has.
    [SerializeField] private Image fillImage;                           // The image component of the slider.

    [SerializeField] private Color fullHealthColor = Color.green;       // The color the health bar will be when on full health.
    [SerializeField] private Color zeroHealthColor = Color.red;         // The color the health bar will be when on no health.

    private float startingHealth = 0;
    private float currentHealth = 0;

    public void Initialize(float startingHealth, UnityEvent<float> OnHealthChange)
    {
        OnHealthChange.AddListener(UpdateHealth);

        this.startingHealth = startingHealth;

        UpdateHealth(startingHealth);

    }

    private void UpdateHealth(float health)
    {
        currentHealth = health;

        // Set the slider's value appropriately.
        slider.value = currentHealth;

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, currentHealth / startingHealth);
    }
    
}
