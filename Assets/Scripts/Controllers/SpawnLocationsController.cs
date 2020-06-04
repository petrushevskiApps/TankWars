using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocationsController : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnLocations = new List<Transform>();

    private List<Transform> availableLocations = new List<Transform>();

    private void Awake()
    {
        GameManager.OnMatchSetup.AddListener(SetupController);
    }
    private void OnDestroy()
    {
        GameManager.OnMatchSetup.RemoveListener(SetupController);
    }

    private void SetupController(MatchConfiguration arg0)
    {
        availableLocations.Clear();
        
        spawnLocations.ForEach(location => availableLocations.Add(location));
    }

    public Vector3 GetSpawnLocation()
    {
        if (availableLocations.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, availableLocations.Count);
            Vector3 location = availableLocations[index].position;
            availableLocations.RemoveAt(index);
            
            return location;
        }

        return Vector3.zero;
    }
}
