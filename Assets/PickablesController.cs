using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PickablesController : MonoBehaviour
{
    [SerializeField] GameObject healthPackPrefab;
    [SerializeField] GameObject ammoPackPrefab;

    [SerializeField] private int hpMapLimit = 3;
    [SerializeField] private int ammoMapLimit = 3;
    [SerializeField] private float minProximity = 20f;

    // Relocation timer range in seconds
    [SerializeField] private int minRelactionTime = 60;
    [SerializeField] private int maxRelactionTime = 60 * 10;

    [SerializeField]  private List<GameObject> healthPacks;
    [SerializeField]  private List<GameObject> ammoPacks;

    [SerializeField] private List<GameObject> hidingSpots;

    

    private NavMeshAgent navMeshTest;
    private NavMeshPath path;

    private List<List<GameObject>> proximityCheckList = new List<List<GameObject>>();

    private void Awake()
    {
        navMeshTest = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();

        healthPacks = new List<GameObject>();
        ammoPacks = new List<GameObject>();

        proximityCheckList.Add(healthPacks);
        proximityCheckList.Add(ammoPacks);
        proximityCheckList.Add(hidingSpots);
    }
    private void Start()
    {
        InstantiatePickables(healthPackPrefab, hpMapLimit, healthPacks);
        InstantiatePickables(ammoPackPrefab, ammoMapLimit, ammoPacks);
    }

    private void InstantiatePickables(GameObject prefab, int limit, List<GameObject> list)
    {
        for(int i=0; i < limit; i++)
        {
            CreatePickable(prefab, list);
        }
    }
    private void CreatePickable(GameObject prefab, List<GameObject> list)
    {
        Vector3 location = GetLocation();
        
        GameObject pickable = Instantiate(prefab, location, Quaternion.identity, transform);
        pickable.GetComponentInChildren<Pickable>().OnCollected.AddListener(ReActivatePickable);
        list.Add(pickable);
    }

    private void ReActivatePickable(GameObject pickable)
    {
        StartCoroutine(ReactivationTimer(pickable));
    }

    IEnumerator ReactivationTimer(GameObject pickable)
    {
        float realocationSeconds = UnityEngine.Random.Range(minRelactionTime, maxRelactionTime);
        yield return new WaitForSecondsRealtime(realocationSeconds);
        pickable.transform.position = GetLocation();
        
        pickable.SetActive(true);

    }

    // Check if the location of the pickable
    // can be reached by the agents
    private Vector3 GetLocation()
    {
        Vector3 location = CornerCalculator.Instance.GetRandomInWorldCoordinates();
        location.y += 1f;

        if(!CheckProximity(location))
        {
            return GetLocation();
        }

        path = new NavMeshPath();
        navMeshTest.CalculatePath(location, path);

        if(path.status == NavMeshPathStatus.PathComplete)
        {
            return location;
        }

        return GetLocation();
    }

    // Check is the random location to close
    // to other pickables or hiding spots
    private bool CheckProximity(Vector3 location)
    {
        foreach (List<GameObject> list in proximityCheckList)
        {
            foreach (GameObject element in list)
            {
                if (Vector3.Distance(location, element.transform.position) <= minProximity)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
