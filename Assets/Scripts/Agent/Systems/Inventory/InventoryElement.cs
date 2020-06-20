using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryElement : MonoBehaviour
{
    public OnValueChange AmountChanged = new OnValueChange();
    public OnStatusChangeEvent OnStatusChange = new OnStatusChangeEvent();

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

    private void OnEnable()
    {
        SetStatus();
    }
    public void Increase(float value)
    {
        Amount = Mathf.Clamp(Amount + value, 0, Capacity);
        AmountChanged.Invoke(Amount);
        SetStatus();
    }
    public void Decrease(float value)
    {
        Amount = Mathf.Clamp(Amount - value, 0, Capacity);
        AmountChanged.Invoke(Amount);
        SetStatus();
    }

    protected void SetStatus()
    {
        float currentPercent = Amount / Capacity;
        InventoryStatus previousStatus = Status;

        if (currentPercent <= 0)
        {
            Status = InventoryStatus.Empty;
        }
        else if (currentPercent > 0 && currentPercent < 0.5f)
        {
            Status = InventoryStatus.Low;
        }
        else if (currentPercent >= 0.5f && currentPercent < 1f)
        {
            Status = InventoryStatus.Medium;
        }
        else
        {
            Status = InventoryStatus.Full;
        }

        if (previousStatus != Status) OnStatusChange.Invoke(Status);
    }

    public class OnValueChange : UnityEvent<float> { }
    public class OnStatusChangeEvent : UnityEvent<InventoryStatus> { }
}
