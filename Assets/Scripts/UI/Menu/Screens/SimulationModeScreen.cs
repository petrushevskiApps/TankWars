using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationModeScreen : ModeScreen
{
    private new void Awake()
    {
        gameModeType = GameModeTypes.Simulation;
        base.Awake();
    }
}
