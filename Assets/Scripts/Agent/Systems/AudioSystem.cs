using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    [SerializeField] private AudioConfiguration configuration;

    [Header("Agent Audio Sources")]
    [SerializeField] private AudioSource drivingSource;
    [SerializeField] private AudioSource sfxSource;

    // The amount by which the pitch of the engine noises can vary.
    [SerializeField] private float pitchRange = 0.2f;

    // The pitch of the audio source at the start of the scene.
    private float originalPitch;

    private Agent agent;

    private void Awake()
    {
        agent = transform.parent.GetComponent<Agent>();
        originalPitch = drivingSource.pitch;
    }
    private void Start()
    {
        RegisterListeners();
    }
    private void OnDestroy()
    {
        UnregisterListeners();
    }

    private void RegisterListeners()
    {
        agent.Weapon.OnShooting.AddListener(PlayShooting);
        agent.Navigation.OnAgentIdling.AddListener(PlayIdling);
        agent.Navigation.OnAgentMoving.AddListener(PlayDriving);
    }

    private void UnregisterListeners()
    {
        agent.Weapon.OnShooting.RemoveListener(PlayShooting);
        agent.Navigation.OnAgentIdling.RemoveListener(PlayIdling);
        agent.Navigation.OnAgentMoving.AddListener(PlayDriving);
    }

    public void PlayDriving()
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
    public void PlayIdling()
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
    public void PlayShooting()
    {
        AudioClip clip = configuration.Shooting;
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    
    
}
