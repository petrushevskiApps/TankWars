using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Pickable : MonoBehaviour
{
    public OnCollectedEvent OnCollected = new OnCollectedEvent();

    private float timeIn = 0;
    private float timeToCollect = 2f;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Tank"))
        {
            if (timeIn >= timeToCollect)
            {
                ICollector agent = other.gameObject.transform.parent.GetComponent<ICollector>();
                Collected(agent);
            }
            else
            {
                timeIn += Time.deltaTime;
            }
        }
    }

    protected virtual void Collected(ICollector collector)
    {
        OnCollected.Invoke(transform.parent.gameObject);
        transform.parent.gameObject.SetActive(false);
    }

    

    public class OnCollectedEvent : UnityEvent<GameObject>
    {

    }
}