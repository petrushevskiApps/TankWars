using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class UIMatchTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countDown;

    [SerializeField] private Color32 normalColor;
    [SerializeField] private Color32 lowColor;

    private float startTime = 0;

    public void Awake()
    {
        GameManager.Instance.MatchTimer.OnTimerTick.AddListener(UpdateTimer);
    }
    private void OnDestroy()
    {
        GameManager.Instance.MatchTimer.OnTimerTick.RemoveListener(UpdateTimer);
    }
    private void OnEnable()
    {
        Setup(GameManager.Instance.MatchConfiguration);
    }
    private void Setup(MatchConfiguration config)
    {
        startTime = config.matchTime;
        countDown.color = normalColor;
        countDown.fontStyle = FontStyles.Normal;
    }

    private void UpdateTimer(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - (minutes * 60);

        StringBuilder cdString = new StringBuilder();

        if (minutes < 10) cdString.Append("0" + minutes);
        else cdString.Append(minutes.ToString());

        cdString.Append(":");

        if (seconds < 10) cdString.Append("0" + seconds);
        else cdString.Append(seconds.ToString());

        countDown.text = cdString.ToString();

        SetTimerColor(time);
        
    }

    private void SetTimerColor(float currentTime)
    {
        float timeLeftPercents = (currentTime / startTime) * 100;

        if (timeLeftPercents < 20f)
        {
            countDown.color = lowColor;
            countDown.fontStyle = FontStyles.Bold;
        }
    }
}
