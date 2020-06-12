using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesController : MonoBehaviour
{
    [SerializeField] private GameObject deathParticlesPrefab;
    [SerializeField] private List<ParticleSystem> drivingParticles;
    private GameObject deathParticles;

    private void Start()
    {
        SetDeathParticles();
    }

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

    private void SetDeathParticles()
    {
        deathParticles = Instantiate(deathParticlesPrefab, World.Instance.agentsExplosions);
        deathParticles.SetActive(false);
    }

    public void ShowDeathParticles(GameObject agent)
    {
        deathParticles.transform.position = transform.position;
        deathParticles.SetActive(true);

        ParticleSystem.MainModule mainModule = deathParticles.GetComponent<ParticleSystem>().main;
        Destroy(deathParticles, mainModule.duration);
    }
}
