using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Pickable : MonoBehaviour, IDestroyable
{
    [SerializeField] private float timeToCollect = 2f;

    public OnCollectedEvent OnCollected = new OnCollectedEvent();

    public float GetTimeToCollect()
    {
        return timeToCollect;
    }

    public virtual void Collect(ICollector collector)
    {
        OnCollected.Invoke(transform.gameObject);
        transform.gameObject.SetActive(false);
    }

    public void RegisterOnDestroy(UnityAction<GameObject> OnDestroyAction)
    {
        OnCollected.AddListener(OnDestroyAction);
    }

    public class OnCollectedEvent : UnityEvent<GameObject>
    {

    }
}