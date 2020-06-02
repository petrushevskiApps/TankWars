using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PickablesController : MonoBehaviour
{
    [SerializeField] GameObject healthPackPrefab;
    [SerializeField] GameObject ammoPackPrefab;
    [SerializeField] private NavMeshAgent navMeshTest;

    [SerializeField] private int hpMapLimit = 3;
    [SerializeField] private int ammoMapLimit = 3;
    [SerializeField] private float minProximity = 20f;

    // Relocation timer range in seconds
    [SerializeField] private int minRelactionTime = 60;
    [SerializeField] private int maxRelactionTime = 60 * 10;

    [SerializeField]  private List<GameObject> healthPacks;
    [SerializeField]  private List<GameObject> ammoPacks;
    [SerializeField] private List<GameObject> hidingSpots;



    
    private NavMeshPath path;

    private List<List<GameObject>> proximityCheckList = new List<List<GameObject>>();

    private List<Coroutine> Timers = new List<Coroutine>();

    private void Awake()
    {
        GameManager.OnMatchSetup.AddListener(CreatePickables);
        GameManager.OnMatchEnded.AddListener(CleanupPickables);
        
        path = new NavMeshPath();

        healthPacks = new List<GameObject>();
        ammoPacks = new List<GameObject>();

        proximityCheckList.Add(healthPacks);
        proximityCheckList.Add(ammoPacks);
        proximityCheckList.Add(hidingSpots);
    }

    private void OnDestroy()
    {
        GameManager.OnMatchSetup.RemoveListener(CreatePickables);
        GameManager.OnMatchEnded.RemoveListener(CleanupPickables);
    }

    private void CreatePickables(MatchConfiguration configuration)
    {
        InstantiatePickables(healthPackPrefab, hpMapLimit, healthPacks);
        InstantiatePickables(ammoPackPrefab, ammoMapLimit, ammoPacks);
    }

    private void CleanupPickables()
    {
        foreach(Coroutine timer in Timers)
        {
            StopCoroutine(timer);
        }

        foreach(GameObject go in healthPacks)
        {
            Destroy(go);
        }
        foreach (GameObject go in ammoPacks)
        {
            Destroy(go);
        }
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
        pickable.name = pickable.name + "( " + ( list.Count + 1 ) + " )";
        pickable.GetComponentInChildren<Pickable>().OnCollected.AddListener(ReactivatePickable);
        list.Add(pickable);
    }

    private void ReactivatePickable(GameObject pickable)
    {
        Coroutine timer = StartCoroutine(ReactivationTimer(pickable));
        Timers.Add(timer);
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
        Vector3 location = World.Instance.GetRandomLocation();
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
