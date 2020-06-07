using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryAmmo : InventoryElement
{
    public override void Increase(float value)
    {
        Amount = Mathf.Clamp(Amount + value, 0, Capacity);
        AmountChanged.Invoke(Amount);
        SetStatus();
    }

    public override void Decrease(float value)
    {
        Amount = Mathf.Clamp(Amount - 1, 0, Capacity);
        AmountChanged.Invoke(Amount);
        SetStatus();
    }

    public override void SetStatus()
    {
        if (Amount <= 0)
        {
            Status = InventoryStatus.Empty;
        }
        else if (Amount > 0 && Amount <= 5)
        {
            Status = InventoryStatus.Low;
        }
        else if (Amount > 5 && Amount < Capacity)
        {
            Status = InventoryStatus.Medium;
        }
        else if (Amount >= Capacity)
        {
            Status = InventoryStatus.Full;
        }
    }
}
