using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAudioSystem : AudioSystem
{
    private AIAgent agent;

    private void Awake()
    {
        agent = transform.parent.GetComponent<AIAgent>();
    }

    protected override void RegisterListeners()
    {
        agent.Navigation.OnAgentIdling.AddListener(PlayIdling);
        agent.Navigation.OnAgentMoving.AddListener(PlayDriving);
        agent.Weapon.OnShooting.AddListener(PlayShooting);

        agent.OnAgentDeath.AddListener(PlayAgentDeath);
        //agent.GetInventory().OnHealthChange.AddListener(PlayHealthLow);
        agent.Memory.OnUnderFire.AddListener(PlayUnderFire);

        agent.Memory.Enemies.OnDetected.AddListener(EnemyDetected);
        agent.Memory.Enemies.OnRemoved.AddListener(EnemyLost);

        agent.gameObject.GetComponent<RunAway>().OnRunAwayEntered.AddListener(PlayRunAwayEntered);
        agent.gameObject.GetComponent<RunAway>().OnRunAwayExecuted.AddListener(PlayRunAwayCompleted);
        agent.gameObject.GetComponent<Patrol>().OnPatrolExecuted.AddListener(PlayRunAwayCompleted);
        agent.gameObject.GetComponent<EliminateEnemy>().OnEnemyAttacked.RemoveListener(PlayEnemyAttacked);
        agent.gameObject.GetComponent<EliminateEnemy>().OnEnemyKilled.RemoveListener(PlayEnemyKilled);

    }

    protected override void UnregisterListeners()
    {
        agent.Navigation.OnAgentIdling.RemoveListener(PlayIdling);
        agent.Navigation.OnAgentMoving.RemoveListener(PlayDriving);
        agent.Weapon.OnShooting.RemoveListener(PlayShooting);

        agent.OnAgentDeath.RemoveListener(PlayAgentDeath);
        //agent.GetInventory().OnHealthChange.AddListener(PlayHealthLow);
        agent.Memory.OnUnderFire.RemoveListener(PlayUnderFire);

        agent.Memory.Enemies.OnDetected.RemoveListener(EnemyDetected);
        agent.Memory.Enemies.OnRemoved.RemoveListener(EnemyLost);

        agent.gameObject.GetComponent<RunAway>().OnRunAwayEntered.RemoveListener(PlayRunAwayEntered);
        agent.gameObject.GetComponent<RunAway>().OnRunAwayExecuted.RemoveListener(PlayRunAwayCompleted);
        agent.gameObject.GetComponent<Patrol>().OnPatrolExecuted.RemoveListener(PlayRunAwayCompleted);
        agent.gameObject.GetComponent<EliminateEnemy>().OnEnemyAttacked.RemoveListener(PlayEnemyAttacked);
        agent.gameObject.GetComponent<EliminateEnemy>().OnEnemyKilled.RemoveListener(PlayEnemyKilled);
    }

    protected void EnemyDetected()
    {
        AudioClip clip = configuration.EnemyDetected.GetRandomClip();

        if (!enemyVoiceLock && agent.Memory.Enemies.GetValidDetectedCount() > 1)
        {
            enemyVoiceLock = true;
            voiceAudioLock = false;
            clip = configuration.EnemiesDetected.GetRandomClip();
        }

        PlayVoice(clip);
    }
    protected void EnemyLost()
    {
        AudioClip clip = configuration.EnemyLost.GetRandomClip();
        PlayVoice(clip);
    }

    protected void PlayRunAwayCompleted()
    {
        AudioClip clip = configuration.onPatrolComplete.GetRandomClip();
        PlayVoice(clip);
    }

    protected void PlayRunAwayEntered()
    {
        AudioClip clip = configuration.runAwayVoice.GetRandomClip();
        PlayVoice(clip);
    }

}
