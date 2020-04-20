using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerCalculator : MonoBehaviour
{
    [SerializeField] private List<Transform> corners;

    public float minX = Mathf.Infinity;
    public float maxX = 0;
    public float minZ = Mathf.Infinity;
    public float maxZ = 0;

    public static CornerCalculator Instance;

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

    public Vector3 GetRandomInWorldCoordinates()
    {
        float x = Random.Range(minX, maxX);
        float z = Random.Range(minZ, maxZ);

        return new Vector3(x, corners[0].position.y, z);
    }
}
