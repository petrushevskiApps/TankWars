using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_ExplosionParticles;        // The particle system the will play when the tank is destroyed.
    [SerializeField] private AudioSource m_ExplosionAudio;               // The audio source to play when the tank explodes.

    private void OnEnable()
    {
        // Play the particle system of the tank exploding.
        m_ExplosionParticles.Play();

        // Play the tank explosion sound effect.
        m_ExplosionAudio.Play();
    }
}
