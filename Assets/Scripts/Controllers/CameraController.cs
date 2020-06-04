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

    private List<Agent> groupTargets;
    private Player playerTarget;
    private CinemachineVirtualCamera currentCamera;

    private AudioListener cameraAudioListener;

    private new void Awake()
    {
        base.Awake();

        cameraAudioListener = GetComponent<AudioListener>();

        GameManager.OnMatchSetup.AddListener(SetupMatchCamera);
        GameManager.OnMatchStarted.AddListener(ActivateGameCamera);
        GameManager.OnMatchExited.AddListener(ResetCameraController);
    }
    private void OnDestroy()
    {
        GameManager.OnMatchSetup.RemoveListener(SetupMatchCamera);
        GameManager.OnMatchStarted.RemoveListener(ActivateGameCamera);
        GameManager.OnMatchExited.RemoveListener(ResetCameraController);
    }

    private void ResetCameraController()
    {
        overviewTargetGroup.m_Targets = new CinemachineTargetGroup.Target[0];

        groupTargets.Clear();
        playerTarget = null;

        if (currentCamera != null)
        {
            currentCamera.gameObject.SetActive(false);
            currentCamera = null;
        }
    }

    private void SetupMatchCamera(MatchConfiguration configuration)
    {
        if (configuration.CameraMode == CameraMode.FollowPlayer)
        {
            SetPlayerTarget();
            SetupFollowCamera(playerTarget);
        }
        else
        {
            SetGroupTargets();

            if (configuration.CameraMode == CameraMode.Overview)
            {
                SetupOverviewCamera();
            }
            if (configuration.CameraMode == CameraMode.FollowOne)
            {
                SetupFollowCamera(GetRandomTarget());
            }
        }
    }



    public void ActivateUICamera()
    {
        ToggleUiCamera(true);
        ToggleMatchCamera(false);
    }

    private void ActivateGameCamera(MatchConfiguration configuration)
    {
        ToggleUiCamera(false);
        ToggleMatchCamera(true);
    }

    private void SetupOverviewCamera()
    {
        cameraAudioListener.enabled = true;
        
        groupTargets.ForEach(target =>
        {
            overviewTargetGroup.AddMember(target.gameObject.transform, 1, 0);
        });

        currentCamera = overviewCamera;
    }

    private void SetupFollowCamera(Agent target)
    {
        target.GetComponent<AudioListener>().enabled = true;

        followCamera.Follow = target.cameraTracker.transform;
        followCamera.LookAt = target.cameraTracker.transform;
        
        currentCamera = followCamera;
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

    private void SetPlayerTarget()
    {
        playerTarget = FindObjectOfType<Player>();
    }
    private void SetGroupTargets()
    {
        groupTargets = FindObjectsOfType<Agent>().ToList();
    }
    private Agent GetRandomTarget()
    {
        return groupTargets[Random.Range(0, groupTargets.Count)];
    }
}
