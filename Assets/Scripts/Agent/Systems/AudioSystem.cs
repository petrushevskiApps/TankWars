using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioSystem : MonoBehaviour
{
    [SerializeField] protected AudioConfiguration configuration;

    [Header("Agent Audio Sources")]
    [SerializeField] protected AudioSource drivingSource;
    [SerializeField] protected AudioSource sfxSource;

    protected Agent agent;
    
    public float pitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.
    private float originalPitch;              // The pitch of the audio source at the start of the scene.

    private void Start()
    {
        RegisterListeners();

        originalPitch = drivingSource.pitch;
    }
    private void OnDestroy()
    {
        UnregisterListeners();
    }

    protected virtual void RegisterListeners()
    {
        agent.Weapon.OnShooting.AddListener(PlayShooting);
    }

    protected virtual void UnregisterListeners()
    {
        agent.Weapon.OnShooting.RemoveListener(PlayShooting);
    }

    protected void PlayDriving()
    {
        // if the tank is moving and if the idling clip is currently playing...
        if (drivingSource.clip == configuration.EngineIdle)
        {
            // ... change the clip to driving and play.
            drivingSource.clip = configuration.EngineDriving;
            drivingSource.pitch = UnityEngine.Random.Range(originalPitch - pitchRange, originalPitch + pitchRange);
            drivingSource.Play();
        }
    }
    protected void PlayIdling()
    {
        // if the audio source is currently playing the driving clip...
        if (drivingSource.clip == configuration.EngineDriving)
        {
            // ... change the clip to idling and play it.
            drivingSource.clip = configuration.EngineIdle;
            drivingSource.pitch = UnityEngine.Random.Range(originalPitch - pitchRange, originalPitch + pitchRange);
            drivingSource.Play();
        }
    }
    protected void PlayShooting()
    {
        AudioClip clip = configuration.Shooting;
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    
    
}
