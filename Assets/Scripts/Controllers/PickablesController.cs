using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PickablesController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject healthPackPrefab;
    [SerializeField] GameObject ammoPackPrefab;

    [Header("Procedural Settings")]
    [SerializeField] private NavMeshAgent navMeshTest;
    [SerializeField] private int hpMapLimit = 3;
    [SerializeField] private int ammoMapLimit = 3;
    [SerializeField] private float minProximity = 20f;

    [Header("Relocation Timer (seconds)")]
    [SerializeField] private int minRelocationTime = 60;
    [SerializeField] private int maxRelocationTime = 60 * 10;

    [Header("Proximity Check Lists")]
    [SerializeField]  private List<GameObject> pickables;

    
    private NavMeshPath path;

    private List<Coroutine> Timers = new List<Coroutine>();

    private void Awake()
    {
        GameManager.OnMatchSetup.AddListener(SetupController);
        GameManager.OnMatchExited.AddListener(CleanupPickables);
        
        path = new NavMeshPath();
        pickables = new List<GameObject>();
    }

    private void OnDestroy()
    {
        GameManager.OnMatchSetup.RemoveListener(SetupController);
        GameManager.OnMatchExited.RemoveListener(CleanupPickables);
    }

    private void SetupController(MatchConfiguration configuration)
    {
        pickables.Clear();

        CreatePickables();
    }

    private void CreatePickables()
    {
        InstantiatePickables(healthPackPrefab, hpMapLimit);
        InstantiatePickables(ammoPackPrefab, ammoMapLimit);
    }

    private void CleanupPickables()
    {
        Timers.ForEach(timer => StopCoroutine(timer));

        pickables.ForEach(pack => Destroy(pack));
    }

    private void InstantiatePickables(GameObject typePrefab, int typeLimit)
    {
        for(int i=0; i < typeLimit; i++)
        {
            GameObject pickable = Instantiate(typePrefab, Vector3.zero, Quaternion.identity, transform);
            SetupPickable(pickable);
            pickables.Add(pickable);
        }
    }

    private void SetupPickable(GameObject pickable)
    {
        pickable.transform.position = GetLocation();
        pickable.name = pickable.name + "( " + ( pickables.Count + 1 ) + " )";
        pickable.GetComponentInChildren<Pickable>().OnCollected.AddListener(ReactivatePickable);
        pickable.SetActive(true);
    }

    private void ReactivatePickable(GameObject pickable)
    {
        pickable.GetComponentInChildren<Pickable>().OnCollected.RemoveListener(ReactivatePickable);
        Coroutine timer = StartCoroutine(ReactivationTimer(pickable));
        Timers.Add(timer);
    }

    IEnumerator ReactivationTimer(GameObject pickable)
    {
        float relocationSeconds = UnityEngine.Random.Range(minRelocationTime, maxRelocationTime);
        yield return new WaitForSecondsRealtime(relocationSeconds);
        SetupPickable(pickable);
    }

    // Check if the location for the pickable
    // can be reached by navmesh agents
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
        foreach (GameObject element in pickables)
        {
            if (Vector3.Distance(location, element.transform.position) <= minProximity)
            {
                return false;
            }
        }

        return true;
    }
}
