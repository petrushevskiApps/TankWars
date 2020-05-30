using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup overviewTargetGroup;

    [SerializeField] private CinemachineVirtualCamera uiCamera;
    [SerializeField] private CinemachineVirtualCamera overviewCamera;
    [SerializeField] private CinemachineVirtualCamera followCamera;

    public static CameraController Instance;

    private GameObject cameraTarget;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ToggleUICamera(bool status)
    {
        World.Instance.ToggleUiTank(status);
        uiCamera.gameObject.SetActive(status);
        overviewCamera.gameObject.SetActive(false);
        followCamera.gameObject.SetActive(false);
    }
    public void GameCamera(CameraMode cameraMode)
    {
        ToggleUICamera(false);

        if (cameraMode == CameraMode.Overview)
        {
            SetupOverviewCamera();
        }
        else if(cameraMode == CameraMode.FollowPlayer)
        {
            cameraTarget = GameManager.Instance.AgentsController.GetPlayer().cameraTracker;
            SetupFollowCamera();
        }
        else if (cameraMode == CameraMode.FollowOne)
        {
            cameraTarget = GameManager.Instance.AgentsController.GetRandomAgent().cameraTracker;
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


    public void SetCameraTargets()
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
