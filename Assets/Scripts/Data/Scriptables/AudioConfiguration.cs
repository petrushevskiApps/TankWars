using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This Scriptable object holds references
 * to all different audio types ( audio data ).
 */
[CreateAssetMenu(fileName = "AudioConfiguration", menuName = "Data/AudioConfiguration", order = 1)]
public class AudioConfiguration : ScriptableObject
{
    [Header("Driving")]
    public AudioClip EngineIdle;
    public AudioClip EngineDriving;

    [Header("Shooting")]
    public AudioClip Shooting;

}