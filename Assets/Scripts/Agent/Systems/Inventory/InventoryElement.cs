using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class InventoryElement : MonoBehaviour
{
    [SerializeField] private float amount = 100;
    [SerializeField] private float capacity = 100;

    public InventoryStatus Status { get; protected set; }

    public float Amount
    {
        get => amount;
        protected set => amount = value;
    }
    public float Capacity
    {
        get => capacity;
        protected set => capacity = value;
    }

    [HideInInspector]
    public OnValueChange AmountChanged = new OnValueChange();

    private void OnEnable()
    {
        SetStatus();
    }

    public abstract void Increase(float value);

    public abstract void Decrease(float value);

    public abstract void SetStatus();

    public class OnValueChange : UnityEvent<float> { }
}
