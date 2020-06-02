using Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioSystem : AudioSystem
{
    private Player player;
    private PlayerMovement moveController;

    private void Awake()
    {
        player = transform.parent.GetComponent<Player>();
        moveController = transform.parent.GetComponent<PlayerMovement>();
    }

    protected override void RegisterListeners()
    {
        moveController.OnAgentMoving.AddListener(PlayDriving);
        moveController.OnAgentIdling.AddListener(PlayIdling);

        player.GetWeapon().OnShooting.AddListener(PlayShooting);
    }

    protected override void UnregisterListeners()
    {
        moveController.OnAgentMoving.RemoveListener(PlayDriving);
        moveController.OnAgentIdling.RemoveListener(PlayIdling);

        player.GetWeapon().OnShooting.RemoveListener(PlayShooting);
    }

}
