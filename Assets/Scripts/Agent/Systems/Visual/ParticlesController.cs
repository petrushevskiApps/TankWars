using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesController : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> drivingParticles;

    public void PlayDrivingParticles()
    {
        drivingParticles.ForEach(ps => 
        {
            if (!ps.isPlaying) ps.Play();
        });
    }
    public void StopDrivingParticles()
    {
        drivingParticles.ForEach(ps =>
        {
            if (ps.isPlaying)
            {
                ps.Stop();
            }
        });

    }
}
