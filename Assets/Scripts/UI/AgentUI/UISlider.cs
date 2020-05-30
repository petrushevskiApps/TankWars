using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISlider : MonoBehaviour
{
    [SerializeField] private Slider slider;                              // The slider to represent how much health the tank currently has.
    [SerializeField] private Image fillImage;                           // The image component of the slider.

    [SerializeField] private Color fullSliderColor = Color.green;       // The color the health bar will be when on full health.
    [SerializeField] private Color zeroSliderColor = Color.red;         // The color the health bar will be when on no health.

    protected float startingValue = 0;
    private float currentValue = 0;

    public void Initialize(float startingAmmo,float capacity, UnityEvent<float> OnValueChange)
    {
        OnValueChange.AddListener(UpdateSlider);

        slider.minValue = 0;
        slider.maxValue = capacity;

        startingValue = startingAmmo;

        UpdateSlider(startingAmmo);

    }

    protected void UpdateSlider(float value)
    {
        currentValue = value;

        // Set the slider's value appropriately.
        slider.value = currentValue;

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        fillImage.color = Color.Lerp(zeroSliderColor, fullSliderColor, currentValue / startingValue);
    }
}
