using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : Singleton<World>
{
    
    [SerializeField] private List<Transform> corners;

    private float minX = Mathf.Infinity;
    private float maxX = 0;
    private float minZ = Mathf.Infinity;
    private float maxZ = 0;

    public Transform worldLocations;
    public Transform shellsParent;
    public Transform destroyedAgents;
    public GameObject uiTank;
    
    private new void Awake()
    {
        base.Awake();

        GameManager.OnMatchStarted.AddListener(OnMatchStart);
        GameManager.OnMatchExited.AddListener(OnMatchExit);

        SetupWorldCorners();
    }

    private void OnDestroy()
    {
        GameManager.OnMatchStarted.AddListener(OnMatchStart);
        GameManager.OnMatchExited.RemoveListener(CleanupWorld);
    }

    private void SetupWorldCorners()
    {
        foreach (Transform corner in corners)
        {
            if (corner.position.x < minX)
            {
                minX = corner.position.x;
            }

            if (corner.position.x > maxX)
            {
                maxX = corner.position.x;
            }

            if (corner.position.z < minZ)
            {
                minZ = corner.position.z;
            }

            if (corner.position.z > maxZ)
            {
                maxZ = corner.position.z;
            }

        }
    }
    
    public Vector3 GetRandomLocation()
    {
        float x = UnityEngine.Random.Range(minX, maxX);
        float z = UnityEngine.Random.Range(minZ, maxZ);

        return new Vector3(x, corners[0].position.y, z);
    }

    public Vector3 ClampToWorld(Vector3 location)
    {
        location.x = Mathf.Clamp(location.x, minX, maxX);
        location.z = Mathf.Clamp(location.z, minZ, maxZ);
        location.y = 0f;
        return location;
    }
    private void OnMatchStart(MatchConfiguration arg0)
    {
        uiTank.SetActive(false);
    }

    private void OnMatchExit()
    {
        uiTank.SetActive(true);
        CleanupWorld();
    }

    public void CleanupWorld()
    {
        Clean(worldLocations);
        Clean(shellsParent);
        Clean(destroyedAgents);
    }

    public void Clean(Transform parent)
    {
        for(int i=0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    public Vector3 CreateLocation(GameObject agent, Vector3 direction, float minForward, float maxForward)
    {
        // Add range to direction
        direction *= UnityEngine.Random.Range(minForward, maxForward);

        // Find up direction of agent
        Vector3 upDirection = agent.transform.up.normalized;

        // Get side direction
        Vector3 sideDirection = Vector3.Cross(direction, upDirection);

        sideDirection *= (UnityEngine.Random.Range(0f, 1f) <= 0.5 ? -1 : 1);

        // Add side direction to forward
        direction += UnityEngine.Random.Range(minForward, maxForward) * sideDirection;

        // Find location in opposite direction of the attack
        Vector3 runToLocation = direction + agent.transform.position;

        return runToLocation;
    }

    public Vector3 CreateRunAwayLocation(GameObject agent, Vector3 direction, float minForward, float maxForward)
    {
        // Get opposite direction with magnitude
        direction *= (-UnityEngine.Random.Range(minForward, maxForward));

        // Find up direction of agent
        Vector3 upDirection = agent.transform.up.normalized;

        // Get side direction
        Vector3 sideDirection = Vector3.Cross(direction, upDirection);

        sideDirection *= (UnityEngine.Random.Range(0f, 1f) <= 0.5 ? -1 : 1);

        // Add side direction to forward
        direction += sideDirection;

        // Find location in opposite direction of the attack
        Vector3 runToLocation = direction + agent.transform.position;

        return runToLocation;
    }
}
