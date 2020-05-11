using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocationsController : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnLocations = new List<Transform>();

    private List<Transform> availableLocations = new List<Transform>();

    private void Awake()
    {
        foreach(Transform location in spawnLocations)
        {
            availableLocations.Add(location);
        }
    }

    public Transform GetSpawnLocation()
    {
        if(availableLocations.Count > 0)
        {
            Transform location = availableLocations[Random.Range(0, availableLocations.Count)];
            availableLocations.Remove(location);
            return location;
        }

        return null;
    }
}
