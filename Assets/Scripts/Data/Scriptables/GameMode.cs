﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This Scriptable object holds reference to
 * all available match configurations in game.
 */
[CreateAssetMenu(fileName = "GameMode", menuName = "Data/GameMode", order = 1)]
public class GameMode : ScriptableObject
{
    public List<MatchConfiguration> matchConfigs = new List<MatchConfiguration>();
}
