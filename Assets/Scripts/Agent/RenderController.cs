using System.Collections.Generic;
using UnityEngine;

public class RenderController : MonoBehaviour
{
    [SerializeField] private GameObject deathParticles;
    private GameObject particles;

    [SerializeField] private List<Renderer> renderMaterials;
    private void Awake()
    {
        SetParticles();
    }
    private void SetParticles()
    {
        particles = Instantiate(deathParticles);
        particles.SetActive(false);
    }
    public void ShowParticles()
    {
        particles.transform.position = transform.position;
        particles.SetActive(true);
    }
    public void SetTeamColor(Material color)
    {
        foreach(Renderer renderer in renderMaterials)
        {
            renderer.material = color;
        }
    }
}