using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerModeScreen : ModeScreen
{
    private new void Awake()
    {
        gameModeType = GameModeTypes.Player;
        base.Awake();
    }
}
