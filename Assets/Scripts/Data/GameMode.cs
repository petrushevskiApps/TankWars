using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameMode", menuName = "Data/GameMode", order = 1)]
public class GameMode : ScriptableObject
{
    public List<MatchConfiguration> matchConfigs = new List<MatchConfiguration>();
}
