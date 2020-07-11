using System;
using System.Collections;
using UnityEngine;

public class TimerElement : InventoryElement
{
    [SerializeField] private float increaseTimeFactor = 1;
    [SerializeField] private float decreaseTimeFactor = 2;

    private Coroutine RefillTimer;
    private Coroutine UseTimer;

    public void Refill()
    {
        if (RefillTimer != null) return;
        StopUse();
        RefillTimer = StartCoroutine(Timer(Increase, Capacity, increaseTimeFactor));
    }
    public void StopRefill()
    {
        if (RefillTimer != null)
        {
            StopCoroutine(RefillTimer);
            RefillTimer = null;
        }
    }

    public void Use()
    {
        if (UseTimer != null) return;
        StopRefill();
        UseTimer = StartCoroutine(Timer(Decrease, Amount, decreaseTimeFactor));
    }

    public void StopUse()
    {
        if(UseTimer != null)
        {
            StopCoroutine(UseTimer);
            UseTimer = null;
        }
    }


    IEnumerator Timer(Action<float> action, float startTime, float factor)
    {
        float timeLeft = startTime;

        while(timeLeft > 0f)
        {
            float decrease = Time.deltaTime * factor;
            timeLeft -= decrease;
            action.Invoke(decrease);
            yield return new WaitForSeconds(0.001f);
        }

        yield return null;
    }
}
