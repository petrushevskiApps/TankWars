using System.Collections.Generic;
using UnityEngine;

public class RenderController : MonoBehaviour
{
    [SerializeField] private GameObject deathParticles;
    private GameObject particles;

    [SerializeField] private List<Renderer> renderMaterials;

    private void Start()
    {
        SetParticles();
    }

    private void SetParticles()
    {
        particles = Instantiate(deathParticles, World.Instance.agentsExplosions);
        particles.SetActive(false);
    }
    public void ShowParticles()
    {
        particles.transform.position = transform.position;
        particles.SetActive(true);

        ParticleSystem.MainModule mainModule = particles.GetComponent<ParticleSystem>().main;
        Destroy(particles, mainModule.duration);
    }
    public void SetTeamColor(Material color)
    {
        foreach(Renderer renderer in renderMaterials)
        {
            renderer.material = color;
        }
    }
}