using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioSystem : MonoBehaviour
{
    [SerializeField] protected AudioConfiguration configuration;

    [SerializeField] protected AudioSource drivingSource;
    [SerializeField] protected AudioSource sfxSource;

    [SerializeField] protected AudioSource voiceSource;
    
    protected bool voiceAudioLock = false;

    protected bool enemyVoiceLock = false;

    public float m_PitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.
    private float originalPitch;              // The pitch of the audio source at the start of the scene.

    private void Start()
    {
        RegisterListeners();

        // Store the original pitch of the audio source.
        originalPitch = drivingSource.pitch;
    }
    private void OnDestroy()
    {
        UnregisterListeners();
    }

    protected abstract void RegisterListeners();

    protected abstract void UnregisterListeners();

    

    protected void PlayDriving()
    {
        // Otherwise if the tank is moving and if the idling clip is currently playing...
        if (drivingSource.clip == configuration.EngineIdle)
        {
            // ... change the clip to driving and play.
            drivingSource.clip = configuration.EngineDriving;
            drivingSource.pitch = UnityEngine.Random.Range(originalPitch - m_PitchRange, originalPitch + m_PitchRange);
            drivingSource.Play();
        }
    }
    protected void PlayIdling()
    {
        // ... and if the audio source is currently playing the driving clip...
        if (drivingSource.clip == configuration.EngineDriving)
        {
            // ... change the clip to idling and play it.
            drivingSource.clip = configuration.EngineIdle;
            drivingSource.pitch = UnityEngine.Random.Range(originalPitch - m_PitchRange, originalPitch + m_PitchRange);
            drivingSource.Play();
        }
    }
    protected void PlayShooting()
    {
        AudioClip clip = configuration.Shooting;
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    protected void PlayEnemyKilled()
    {
        AudioClip clip = configuration.EnemyKilled.GetRandomClip();
        PlayVoice(clip);
    }

    protected void PlayEnemyAttacked()
    {
        AudioClip clip = configuration.EnemyAttack.GetRandomClip();
        PlayVoice(clip);
    }

    protected void PlayHealthLow(float health)
    {
        AudioClip clip = configuration.onHealthLowVoice.GetRandomClip();
        PlayVoice(clip);
    }

    protected void PlayAgentDeath(GameObject arg0)
    {
        AudioClip clip = configuration.OnDeath.GetRandomClip();
        PlayVoice(clip);
    }

    protected void PlayUnderFire()
    {
        AudioClip clip = configuration.underAttackVoice.GetRandomClip();
        PlayVoice(clip);
    }

    protected void PlayVoice(AudioClip clip)
    {
        //if (!voiceAudioLock)
        //{
        //    voiceAudioLock = true;
        //    StartCoroutine(ResetVoiceLock());
        //    voiceSource.clip = clip;
        //    voiceSource.Play();
        //}
        voiceSource.PlayOneShot(clip);
    }



    protected IEnumerator ResetVoiceLock()
    {
        yield return new WaitForSeconds(5f);
        voiceAudioLock = false;
    }
}
