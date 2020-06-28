using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Pickable : MonoBehaviour, IDestroyable
{
    public float amountToCollect = 0;
    public float timeToCollect = 2f;

    public OnCollectedEvent OnCollected = new OnCollectedEvent();

    [SerializeField] private GameObject canvas;
    [SerializeField] private Image collectingProgress;
    

    private ICollector collector;
    private Coroutine CollectingCo;

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Tank"))
        {
            canvas.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Tank"))
        {
            canvas.SetActive(false);
        }
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

        canvas.SetActive(false);
        collector = null;
        collectingProgress.fillAmount = 0;
    }

    IEnumerator Collecting()
    {
        float currentTime = 0f;

        while(currentTime < timeToCollect && collector != null)
        {
            currentTime += Time.deltaTime;
            collectingProgress.fillAmount = currentTime * ( 1 / timeToCollect );
            yield return new WaitForSeconds(0.001f);
        }

        if(collector != null)
        {
            Collect(collector);
        }
       
    }

    protected virtual void Collect(ICollector collector)
    {
        OnCollected.Invoke(gameObject);
        
        gameObject.SetActive(false);
        this.collector = null;
    }

    public void RegisterOnDestroy(UnityAction<GameObject> OnDestroyAction)
    {
        OnCollected.AddListener(OnDestroyAction);
    }
    public void UnregisterOnDestroy(UnityAction<GameObject> OnDestroyAction)
    {
        OnCollected.RemoveListener(OnDestroyAction);
    }
    public class OnCollectedEvent : UnityEvent<GameObject> { }
}