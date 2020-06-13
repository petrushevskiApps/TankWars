using System;
using System.Collections;
using UnityEngine;

public class InventoryTime : InventoryElement
{
    [SerializeField] private float increaseTimeFactor = 1;
    [SerializeField] private float decreaseTimeFactor = 2;

    private Coroutine IncreaseTimer;
    private Coroutine DecreaseTimer;

    public void Refill()
    {
        if (IncreaseTimer != null) return;
        DecreaseStop();
        IncreaseTimer = StartCoroutine(Timer(Increase, Capacity, increaseTimeFactor));
    }
    public void IncreaseStop()
    {
        if (IncreaseTimer != null)
        {
            StopCoroutine(IncreaseTimer);
            IncreaseTimer = null;
        }
    }

    public void Use()
    {
        if (DecreaseTimer != null) return;
        IncreaseStop();
        DecreaseTimer = StartCoroutine(Timer(Decrease, Amount, decreaseTimeFactor));
    }

    public void DecreaseStop()
    {
        if(DecreaseTimer != null)
        {
            StopCoroutine(DecreaseTimer);
            DecreaseTimer = null;
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
