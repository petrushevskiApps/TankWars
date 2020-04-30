using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public Slider slider;                              // The slider to represent how much health the tank currently has.
    public Image fillImage;                           // The image component of the slider.
    public Color fullHealthColor = Color.green;       // The color the health bar will be when on full health.
    public Color zeroHealthColor = Color.red;         // The color the health bar will be when on no health.

    private float startingHealth = 0;
    private float currentHealth = 0;

    public void Initialize(float startingHealth)
    {
        this.startingHealth = startingHealth;
        currentHealth = startingHealth;

        UpdateHealth();

    }
    public void SetHealth(float health)
    {
        currentHealth = health;
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        // Set the slider's value appropriately.
        slider.value = currentHealth;

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, currentHealth / startingHealth);
    }
    
}
