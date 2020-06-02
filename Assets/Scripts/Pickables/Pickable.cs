using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Pickable : MonoBehaviour, IDestroyable
{
    [SerializeField] private Image collectingProgress;
    [SerializeField] private float timeToCollect = 2f;

    public OnCollectedEvent OnCollected = new OnCollectedEvent();

    private ICollector collector;
    private Coroutine CollectingCo;

    public float GetTimeToCollect()
    {
        return timeToCollect;
    }

    public void StartCollecting(ICollector collector)
    {
        if (this.collector != null) return;

        this.collector = collector;
        CollectingCo = StartCoroutine(Collecting());
    }

    public void StopCollecting()
    {
        if(CollectingCo != null)
        {
            StopCoroutine(CollectingCo);
            CollectingCo = null;
        }

        collector = null;
        collectingProgress.fillAmount = 0;
    }

    IEnumerator Collecting()
    {
        float currentTime = 0f;

        while(currentTime < timeToCollect)
        {
            currentTime += Time.deltaTime;
            collectingProgress.fillAmount = currentTime * ( 1 / timeToCollect );
            yield return new WaitForEndOfFrame();
        }
        
        Collect(collector);
    }
    protected virtual void Collect(ICollector collector)
    {
        OnCollected.Invoke(transform.gameObject);
        
        transform.gameObject.SetActive(false);
        this.collector = null;
    }

    public void RegisterOnDestroy(UnityAction<GameObject> OnDestroyAction)
    {
        OnCollected.AddListener(OnDestroyAction);
    }

    public class OnCollectedEvent : UnityEvent<GameObject> { }
}