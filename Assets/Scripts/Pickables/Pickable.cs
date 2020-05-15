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
        if (isCollected) return;

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
    private void OnTriggerExit(Collider other)
    {
        timeIn = 0;
    }

    protected virtual void Collected(ICollector collector)
    {
        isCollected = true;
        OnCollected.Invoke(transform.parent.gameObject);
        transform.parent.gameObject.SetActive(false);
    }

    

    public class OnCollectedEvent : UnityEvent<GameObject>
    {

    }
}