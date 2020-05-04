using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickablesController : MonoBehaviour
{
    [SerializeField] GameObject healthPackPrefab;
    [SerializeField] GameObject ammoPackPrefab;

    [SerializeField] private int hpMapLimit = 3;
    [SerializeField] private int ammoMapLimit = 3;

    [SerializeField]  private List<GameObject> healthPacks;
    [SerializeField]  private List<GameObject> ammoPacks;

    private void Start()
    {
        healthPacks = new List<GameObject>();
        ammoPacks = new List<GameObject>();

        InstantiatePickables(healthPackPrefab, hpMapLimit, healthPacks);
        InstantiatePickables(ammoPackPrefab, ammoMapLimit, ammoPacks);
    }

    private void InstantiatePickables(GameObject prefab, int limit, List<GameObject> list)
    {
        for(int i=0; i < limit; i++)
        {
            Vector3 location = CornerCalculator.Instance.GetRandomInWorldCoordinates();
            location.y += 1f;
            GameObject pickable = Instantiate(prefab, location, Quaternion.identity, transform);
            list.Add(pickable);
        }
    }

    //private void Update()
    //{
    //    if(healthPacks.Count < hpMapLimit)
    //    {
    //        StartCoroutine()
    //    }
    //}
}
