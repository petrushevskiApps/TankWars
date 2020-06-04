using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentParticlesSystem : MonoBehaviour
{
    // PARTICLES
    [SerializeField] private ParticleSystem[] particlesSystem; // References to all the particles systems used by the Tanks


    // We grab all the Particle systems child of that Tank to be able to Stop/Play them on Deactivate/Activate
    // It is needed because we move the Tank when spawning it, and if the Particle System is playing while we do that
    // it "think" it move from (0,0,0) to the spawn point, creating a huge trail of smoke
    public void PlayParticles()
    {
        for (int i = 0; i < particlesSystem.Length; ++i)
        {
            particlesSystem[i].Play();
        }
    }

    public void StopParticles()
    {
        // Stop all particle system so it "reset" it's position
        // to the actual one instead of thinking we moved when spawning
        for (int i = 0; i < particlesSystem.Length; ++i)
        {
            particlesSystem[i].Stop();
        }
    }

}
