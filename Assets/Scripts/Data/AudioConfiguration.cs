using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioConfiguration", menuName = "Data/AudioConfiguration", order = 1)]
public class AudioConfiguration : ScriptableObject
{
    [Header("Agent")]
    public AudioData runAwayVoice;
    public AudioData OnDeath;
    public AudioData onHealthLowVoice;
    public AudioData underAttackVoice;
    public AudioData onHealthDecrease;
    public AudioData onPatrolComplete;

    [Header("Enemy")]
    public AudioData EnemyDetected;
    public AudioData EnemiesDetected;
    public AudioData EnemyLost;
    public AudioData EnemyAttack;
    public AudioData EnemyKilled;

    [Header("Target")]
    public AudioData TargetInRange;

    [Header("Driving")]
    public AudioClip EngineIdle;
    public AudioClip EngineDriving;

    [Header("Shooting")]
    public AudioClip Shooting;

}