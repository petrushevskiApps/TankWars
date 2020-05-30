using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : UIScreen
{
    [SerializeField] private Button playerModeButton;
    [SerializeField] private Button simulationModeButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        playerModeButton.onClick.AddListener(OnPlayerMode);
        simulationModeButton.onClick.AddListener(OnSimulationMode);
        controlsButton.onClick.AddListener(OnControls);
        quitButton.onClick.AddListener(OnQuit);
    }
    private void OnDestroy()
    {
        playerModeButton.onClick.RemoveListener(OnPlayerMode);
        simulationModeButton.onClick.RemoveListener(OnSimulationMode);
        controlsButton.onClick.RemoveListener(OnControls);
        quitButton.onClick.RemoveListener(OnQuit);
    }

    private void OnPlayerMode()
    {
        UIController.Instance.ShowScreen<PlayerModeScreen>();
    }
    private void OnSimulationMode()
    {
        UIController.Instance.ShowScreen<SimulationModeScreen>();
    }
    private void OnControls()
    {
        UIController.Instance.ShowScreen<ControlsScreen>();
    }
    private void OnQuit()
    {
        Application.Quit();
    }
}
