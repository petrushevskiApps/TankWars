using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHealth : InventoryElement
{
    public override void Increase(float value)
    {
        Amount = Mathf.Clamp(Amount + value, 0, Capacity);
        AmountChanged.Invoke(Amount);
        SetStatus();
    }
    public override void Decrease(float value)
    {
        Amount = Mathf.Clamp(Amount - value, 0, Capacity);
        AmountChanged.Invoke(Amount);
        SetStatus();
    }

    public override void SetStatus()
    {
        if (Amount < 50)
        {
            Status = InventoryStatus.Low;
        }
        else if (Amount >= 50 && Amount < Capacity)
        {
            Status = InventoryStatus.Medium;
        }
        else
        {
            Status = InventoryStatus.Full;
        }
    }

    
}
