using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Pickable : MonoBehaviour
{
    public OnCollectedEvent OnCollected = new OnCollectedEvent();

    private float timeIn = 0;
    private float timeToCollect = 2f;
    public bool isCollected = false;

    private void OnEnable()
    {
        isCollected = false;
        timeIn = 0;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Tank"))
        {
            if (timeIn >= timeToCollect)
            {
                if(!isCollected)
                {
                    ICollector agent = other.gameObject.transform.parent.GetComponent<ICollector>();
                    Collected(agent);
                    isCollected = true;
                }
                
            }
            else
            {
                timeIn += Time.deltaTime;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        timeIn = 0;
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