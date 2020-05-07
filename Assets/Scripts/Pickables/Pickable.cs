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
                Collected();
            }
            else
            {
                timeIn += Time.deltaTime;
            }
        }
    }

    private void Collected()
    {
        OnCollected.Invoke(transform.parent.gameObject);
        transform.parent.gameObject.SetActive(false);
    }

    

    public class OnCollectedEvent : UnityEvent<GameObject>
    {

    }
}