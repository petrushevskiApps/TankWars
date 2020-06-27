using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryElement : MonoBehaviour
{
    private static int EMPTY_INV_COST = 0;
    private static int VERY_LOW_INV_COST = 2;
    private static int LOW_INV_COST = 4;
    private static int MEDIUM_INV_COST = 6;
    private static int HIGH_INV_COST = 8;
    private static int FULL_INV_COST = 10;


    public OnValueChange AmountChanged { get; private set; } = new OnValueChange();
    
    public UnityEvent OnStatusChange { get; private set; } = new UnityEvent();

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

        if(currentPercent <= 0)
        {
            Status = InventoryStatus.Empty;
        }
        else if (currentPercent > 0 && currentPercent <= 0.3f)
        {
            Status = InventoryStatus.VeryLow;
        }
        else if (currentPercent > 0.3f && currentPercent <= 0.5f)
        {
            Status = InventoryStatus.Low;
        }
        else if (currentPercent > 0.5f && currentPercent <= 0.7f)
        {
            Status = InventoryStatus.Medium;
        }
        else if (currentPercent > 0.7f && currentPercent < 1f)
        {
            Status = InventoryStatus.High;
        }
        else
        {
            Status = InventoryStatus.Full;
        }

        if (previousStatus != Status) OnStatusChange.Invoke();
    }
    /*
	 * Get cost matching the inventory status
	 * Normal Inventory cost means lower statuses
	 * cost more, example Empty inventory has high cost
	 * because it can not be used.
	 */
    public float GetCost()
    {
        switch (Status)
        {
            case InventoryStatus.Empty:     return EMPTY_INV_COST;
            case InventoryStatus.VeryLow:   return VERY_LOW_INV_COST;
            case InventoryStatus.Low:       return LOW_INV_COST;
            case InventoryStatus.Medium:    return MEDIUM_INV_COST;
            case InventoryStatus.High:      return HIGH_INV_COST;
            case InventoryStatus.Full:      return FULL_INV_COST;
            default: return 0;
        }
    }

    /*
	 * Get cost atching the inventory status.
	 * Inverted Inventory cost means higher statuses
	 * cost more, example Empty inventory has low cost
	 */
    public float GetInvertedCost()
    {
        switch (Status)
        {
            case InventoryStatus.Empty:       return FULL_INV_COST - EMPTY_INV_COST;
            case InventoryStatus.VeryLow:     return FULL_INV_COST - VERY_LOW_INV_COST;
            case InventoryStatus.Low:         return FULL_INV_COST - LOW_INV_COST;
            case InventoryStatus.Medium:      return FULL_INV_COST - MEDIUM_INV_COST;
            case InventoryStatus.High:        return FULL_INV_COST - HIGH_INV_COST;
            case InventoryStatus.Full:        return FULL_INV_COST - FULL_INV_COST;
            default: return 0;
        }
    }
    public class OnValueChange : UnityEvent<float> { }
}
