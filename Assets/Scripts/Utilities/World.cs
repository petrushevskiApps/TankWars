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
    public Transform agentsExplosions;
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
        Clean(agentsExplosions);
    }

    public void Clean(Transform parent)
    {
        for(int i=0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}
