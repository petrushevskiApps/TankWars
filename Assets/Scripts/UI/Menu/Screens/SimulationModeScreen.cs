using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationModeScreen : ModeScreen
{
    private new void Awake()
    {
        gameMode = GameModeTypes.Simulation;
        base.Awake();
    }
}
