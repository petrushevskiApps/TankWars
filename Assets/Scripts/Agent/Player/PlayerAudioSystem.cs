using Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioSystem : AudioSystem
{
    private PlayerMovement moveController;

    private void Awake()
    {
        agent = transform.parent.GetComponent<Player>();
        moveController = agent.GetComponent<PlayerMovement>();
    }

    protected override void RegisterListeners()
    {
        base.RegisterListeners();
        moveController.OnAgentMoving.AddListener(PlayDriving);
        moveController.OnAgentIdling.AddListener(PlayIdling);
    }
    protected override void UnregisterListeners()
    {
        base.UnregisterListeners();
        moveController.OnAgentMoving.RemoveListener(PlayDriving);
        moveController.OnAgentIdling.RemoveListener(PlayIdling);
    }
}
