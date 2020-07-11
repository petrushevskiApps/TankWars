using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MatchTimerController : MonoBehaviour
{
    private float matchTime = 0;
    private float timeInMatch = 0;
    private Coroutine Timer;

    public TimerEndEvent OnTimerEnd = new TimerEndEvent();
    public TimerEvent OnTimerTick = new TimerEvent();

    private void Awake()
    {
        GameManager.OnMatchSetup.AddListener(TimerSetup);
        GameManager.OnMatchStarted.AddListener(StartTimer);
        GameManager.OnMatchEnded.AddListener(StopTimer);
    }

    private void OnDestroy()
    {
        GameManager.OnMatchSetup.RemoveListener(TimerSetup);
        GameManager.OnMatchStarted.RemoveListener(StartTimer);
        GameManager.OnMatchEnded.RemoveListener(StopTimer);
    }

    private void TimerSetup(MatchConfiguration configuration)
    {
        matchTime = configuration.matchTime;
        timeInMatch = matchTime;
    }

    private void StartTimer(MatchConfiguration arg0)
    {
        Timer = StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        while(timeInMatch > 0)
        {
            timeInMatch -= Time.deltaTime;
            OnTimerTick.Invoke(timeInMatch);
            yield return new WaitForEndOfFrame();
        }
        OnTimerEnd.Invoke(true);
    }

    private void StopTimer()
    {
        if(Timer != null)
        {
            StopCoroutine(Timer);
            Timer = null;
        }
    }

    public class TimerEvent : UnityEvent<float> { }
    public class TimerEndEvent : UnityEvent<bool> { }
}
