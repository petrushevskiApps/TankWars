using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : UIScreen
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button exitMatchButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener(OnResume);
        controlsButton.onClick.AddListener(OnControl);
        exitMatchButton.onClick.AddListener(OnExit);
    }
    private void OnDestroy()
    {
        resumeButton.onClick.RemoveListener(OnResume);
        controlsButton.onClick.RemoveListener(OnControl);
        exitMatchButton.onClick.RemoveListener(OnExit);
    }

    private void OnControl()
    {
        UIController.Instance.ShowScreen<ControlsScreen>();
    }

    private void OnExit()
    {
        GameManager.Instance.UnpauseGame();
        GameManager.Instance.MatchExited();
    }

    private void OnResume()
    {
        OnBackButton();
    }
    public override void OnBackButton()
    {
        GameManager.Instance.UnpauseGame();
        base.OnBackButton();
    }
}
