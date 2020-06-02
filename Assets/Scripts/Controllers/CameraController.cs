using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private CinemachineTargetGroup overviewTargetGroup;

    [SerializeField] private CinemachineVirtualCamera uiCamera;
    [SerializeField] private CinemachineVirtualCamera overviewCamera;
    [SerializeField] private CinemachineVirtualCamera followCamera;

    private GameObject cameraTarget;

    private new void Awake()
    {
        base.Awake();
        GameManager.OnMatchStarted.AddListener(GameCamera);
    }
    private void OnDestroy()
    {
        GameManager.OnMatchStarted.RemoveListener(GameCamera);
    }

    public void ToggleUICamera(bool status)
    {
        World.Instance.ToggleUiTank(status);
        uiCamera.gameObject.SetActive(status);
        overviewCamera.gameObject.SetActive(false);
        followCamera.gameObject.SetActive(false);
    }
    private void GameCamera(MatchConfiguration configuration)
    {
        ToggleUICamera(false);

        if (configuration.CameraMode == CameraMode.Overview)
        {
            GetComponent<AudioListener>().enabled = true;
            SetupOverviewCamera();
        }
        else if(configuration.CameraMode == CameraMode.FollowPlayer)
        {
            cameraTarget = GameManager.Instance.AgentsController.GetPlayer().cameraTracker;
            SetupFollowCamera();
        }
        else if (configuration.CameraMode == CameraMode.FollowOne)
        {
            cameraTarget = GameManager.Instance.AgentsController.GetCameraTargetAgent().cameraTracker;
            SetupFollowCamera();
        }

    }

    private void SetupOverviewCamera()
    {
        SetCameraTargets();
        overviewCamera.gameObject.SetActive(true);
    }

    private void SetupFollowCamera()
    {
        followCamera.Follow = cameraTarget.transform;
        followCamera.LookAt = cameraTarget.transform;
        followCamera.gameObject.SetActive(true);
    }


    private void SetCameraTargets()
    {
        foreach (List<Agent> team in GameManager.Instance.Teams)
        {
            foreach (Agent agent in team)
            {
                overviewTargetGroup.AddMember(agent.gameObject.transform, 1, 0);
            }
        }
    }

}
