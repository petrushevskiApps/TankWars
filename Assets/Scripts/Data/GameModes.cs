using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameModes", menuName = "Data/GameModes", order = 1)]
public class GameModes : ScriptableObject
{
    public List<MatchConfiguration> simulationConfigs = new List<MatchConfiguration>();
    public List<MatchConfiguration> playerConfigs = new List<MatchConfiguration>();

}
