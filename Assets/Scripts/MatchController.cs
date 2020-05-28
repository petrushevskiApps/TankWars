using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    

    private AgentsController agentsController;

    

    private void Awake()
    {
        agentsController = GetComponent<AgentsController>();
    }

    // Start is called before the first frame update
    

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
