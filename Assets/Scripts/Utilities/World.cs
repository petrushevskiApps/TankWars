using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] private List<Transform> corners;

    private float minX = Mathf.Infinity;
    private float maxX = 0;
    private float minZ = Mathf.Infinity;
    private float maxZ = 0;

    public Transform worldLocations;
    public Transform shellsParent;
    public Transform agentsExplosions;

    public static World Instance;
    
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }

        foreach(Transform corner in corners)
        {
            if(corner.position.x < minX)
            {
                minX = corner.position.x;
            }

            if(corner.position.x > maxX)
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
        float x = Random.Range(minX, maxX);
        float z = Random.Range(minZ, maxZ);

        return new Vector3(x, corners[0].position.y, z);
    }
}
