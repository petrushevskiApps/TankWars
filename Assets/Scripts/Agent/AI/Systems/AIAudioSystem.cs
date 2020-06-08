using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAudioSystem : AudioSystem
{
    private AIAgent aiAgent;

    private void Awake()
    {
        aiAgent = transform.parent.GetComponent<AIAgent>();
        agent = aiAgent;
    }

    protected override void RegisterListeners()
    {
        aiAgent.Navigation.OnAgentIdling.AddListener(PlayIdling);
        aiAgent.Navigation.OnAgentMoving.AddListener(PlayDriving);
    }

    protected override void UnregisterListeners()
    {
        aiAgent.Navigation.OnAgentIdling.RemoveListener(PlayIdling);
        aiAgent.Navigation.OnAgentMoving.RemoveListener(PlayDriving);
    }
}
