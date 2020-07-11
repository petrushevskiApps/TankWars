using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private CinemachineTargetGroup overviewTargetGroup;

    [SerializeField] private CinemachineVirtualCamera uiCamera;
    [SerializeField] private CinemachineVirtualCamera overviewCamera;
    [SerializeField] private CinemachineVirtualCamera followCamera;

    [SerializeField] private GameObject staticTracker;
    [SerializeField] private GameObject deathEffect;

    private List<Agent> aiTargets;
    private Player playerTarget;
    private CinemachineVirtualCamera currentCamera;

    private AudioListener cameraAudioListener;

    private new void Awake()
    {
        base.Awake();

        cameraAudioListener = GetComponent<AudioListener>();

        GameManager.OnMatchSetup.AddListener(SetupMatchCamera);
        //GameManager.OnMatchStarted.AddListener(ActivateGameCamera);
        GameManager.OnMatchEnded.AddListener(SetMatchEndedCamera);
        GameManager.OnMatchExited.AddListener(ResetCameraController);
    }
    private void OnDestroy()
    {
        GameManager.OnMatchSetup.RemoveListener(SetupMatchCamera);
        //GameManager.OnMatchStarted.RemoveListener(ActivateGameCamera);
        GameManager.OnMatchEnded.RemoveListener(SetMatchEndedCamera);
        GameManager.OnMatchExited.RemoveListener(ResetCameraController);
    }

    private void SetMatchEndedCamera()
    {
        if(currentCamera.Equals(followCamera) && currentCamera.Follow == null)
        {
            followCamera.Follow = staticTracker.transform;
            followCamera.LookAt = staticTracker.transform;
        }
    }

    private void ResetCameraController()
    {
        deathEffect.SetActive(false);
        overviewTargetGroup.m_Targets = new CinemachineTargetGroup.Target[0];

        if(aiTargets != null)
        {
            aiTargets.Clear();
        }
        playerTarget = null;

        if (currentCamera != null)
        {
            currentCamera.gameObject.SetActive(false);
            currentCamera = null;
        }
    }

    private void SetupMatchCamera(MatchConfiguration configuration)
    {
        CollectTargets();
        
        if (configuration.CameraMode == CameraMode.Overview)
        {
            SetupOverviewCamera();
        }
        else if (configuration.CameraMode == CameraMode.FollowOne)
        {
            SetupFollowCamera(GetRandomTarget(), true);
        }
        else if (configuration.CameraMode == CameraMode.FollowPlayer)
        {
            SetupFollowCamera(playerTarget, false, true);
        }
        else
        {
            Debug.LogError("No camera mode selected!!!");
        }
        
    }



    public void ActivateUICamera()
    {
        ToggleUiCamera(true);
        ToggleMatchCamera(false);
    }

    private void ActivateGameCamera()
    {
        ToggleUiCamera(false);
        ToggleMatchCamera(true);
    }
    private void SetupOverviewCamera()
    {
        cameraAudioListener.enabled = true;
        
        aiTargets.ForEach(target =>
        {
            overviewTargetGroup.AddMember(target.gameObject.transform, 1, 0);
        });

        currentCamera = overviewCamera;
        
        ActivateGameCamera();
    }

    private void SetupFollowCamera(Agent target, bool resetTracker, bool setDeathMode = false)
    {
        target.GetComponent<AudioListener>().enabled = true;

        followCamera.Follow = target.VisualSystem.Tracker;
        followCamera.LookAt = target.VisualSystem.Tracker;

        currentCamera = followCamera;

        if(resetTracker)
        {
            target.GetComponent<IDestroyable>().RegisterOnDestroy(ResetTracker);
        }
        if(setDeathMode)
        {
            target.GetComponent<IDestroyable>().RegisterOnDestroy(ShowDeathMode);
        }
        ActivateGameCamera();
    }

    // Once the agent ( ai / player ) with camera tracker
    // is destroyed, set tracking random agent.
    private void ResetTracker(GameObject arg0)
    {
        //SetupFollowCamera(GetRandomTarget(), true);
        SetupOverviewCamera();
    }
    private void ShowDeathMode(GameObject arg0)
    {
        deathEffect.SetActive(true);
    }
    private void ToggleUiCamera(bool status)
    {
        if(uiCamera != null)
        {
            uiCamera.gameObject.SetActive(status);
        }
    }

    private void ToggleMatchCamera(bool status)
    {
        if(currentCamera != null)
        {
            currentCamera.gameObject.SetActive(status);
        }
    }

    private void CollectTargets()
    {
        aiTargets = GameManager.Instance.AgentsController.GetAllAgents();
        playerTarget = FindObjectOfType<Player>();
    }
    private Agent GetRandomTarget()
    {
        aiTargets.RemoveAll(agent => agent == null);
        return aiTargets[Random.Range(0, aiTargets.Count)];
    }
}
