using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    [SerializeField] private AudioConfiguration configuration;

    [SerializeField] private AudioSource drivingSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioSource voiceSource;
    private bool voiceAudioLock = false;

    private int enemiesCount = 0;
    private bool enemyVoiceLock = false;

    private AIAgent agent;

    private void Awake()
    {
        agent = transform.parent.GetComponent<AIAgent>();
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
        agent.OnAgentDeath.AddListener(PlayAgentDeath);
        //agent.GetInventory().OnHealthChange.AddListener(PlayHealthLow);
        agent.GetMemory().OnUnderFire.AddListener(PlayUnderFire);

        agent.GetMemory().Enemies.OnDetected.AddListener(EnemyDetected);
        agent.GetMemory().Enemies.OnRemoved.AddListener(EnemyLost);

        agent.gameObject.GetComponent<RunAway>().OnRunAwayEntered.AddListener(PlayRunAwayEntered);
        agent.gameObject.GetComponent<RunAway>().OnRunAwayExecuted.AddListener(PlayRunAwayCompleted);
        agent.gameObject.GetComponent<Patrol>().OnPatrolExecuted.AddListener(PlayRunAwayCompleted);
        agent.gameObject.GetComponent<EliminateEnemy>().OnEnemyAttacked.RemoveListener(PlayEnemyAttacked);
        agent.gameObject.GetComponent<EliminateEnemy>().OnEnemyKilled.RemoveListener(PlayEnemyKilled);

    }

    
    private void UnregisterListeners()
    {
        agent.OnAgentDeath.RemoveListener(PlayAgentDeath);
        //agent.GetInventory().OnHealthChange.AddListener(PlayHealthLow);
        agent.GetMemory().OnUnderFire.RemoveListener(PlayUnderFire);

        agent.GetMemory().Enemies.OnDetected.RemoveListener(EnemyDetected);
        agent.GetMemory().Enemies.OnRemoved.RemoveListener(EnemyLost);

        agent.gameObject.GetComponent<RunAway>().OnRunAwayEntered.RemoveListener(PlayRunAwayEntered);
        agent.gameObject.GetComponent<RunAway>().OnRunAwayExecuted.RemoveListener(PlayRunAwayCompleted);
        agent.gameObject.GetComponent<Patrol>().OnPatrolExecuted.RemoveListener(PlayRunAwayCompleted);
        agent.gameObject.GetComponent<EliminateEnemy>().OnEnemyAttacked.RemoveListener(PlayEnemyAttacked);
        agent.gameObject.GetComponent<EliminateEnemy>().OnEnemyKilled.RemoveListener(PlayEnemyKilled);
    }

    private void PlayEnemyKilled()
    {
        AudioClip clip = configuration.EnemyKilled.GetRandomClip();
        PlayVoice(clip);
    }

    private void PlayEnemyAttacked()
    {
        AudioClip clip = configuration.EnemyAttack.GetRandomClip();
        PlayVoice(clip);
    }

    private void PlayHealthLow(float health)
    {
        AudioClip clip = configuration.onHealthLowVoice.GetRandomClip();
        PlayVoice(clip);
    }

    private void PlayAgentDeath(GameObject arg0)
    {
        AudioClip clip = configuration.OnDeath.GetRandomClip();
        PlayVoice(clip);
    }

    private void EnemyLost()
    {
        AudioClip clip = configuration.EnemyLost.GetRandomClip();
        PlayVoice(clip);
    }

    private void PlayRunAwayCompleted()
    {
        AudioClip clip = configuration.onPatrolComplete.GetRandomClip();
        PlayVoice(clip);
    }

    private void PlayRunAwayEntered()
    {
        AudioClip clip = configuration.runAwayVoice.GetRandomClip();
        PlayVoice(clip);
    }

    private void EnemyDetected()
    {
        AudioClip clip = configuration.EnemyDetected.GetRandomClip();

        if(!enemyVoiceLock && agent.GetMemory().Enemies.GetValidDetectedCount() > 1)
        {
            enemyVoiceLock = true;
            voiceAudioLock = false;
            clip = configuration.EnemiesDetected.GetRandomClip();
        }
        
        PlayVoice(clip);
    }

    private void PlayUnderFire()
    {
        AudioClip clip = configuration.underAttackVoice.GetRandomClip();
        PlayVoice(clip);
    }

    private void PlayVoice(AudioClip clip)
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



    private IEnumerator ResetVoiceLock()
    {
        yield return new WaitForSeconds(5f);
        voiceAudioLock = false;
    }
}
