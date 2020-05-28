using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfiguration", menuName = "Data/LevelConfiguration", order = 1)]
public class LevelConfiguration : ScriptableObject
{
    public LevelModes levelMode;
    public CameraMode cameraMode;

    public TeamsConfig teamsConfig;
}
