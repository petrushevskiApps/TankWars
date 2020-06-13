using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderController : MonoBehaviour
{
    [SerializeField] private List<Renderer> renderMaterials;
    [SerializeField] private GameObject shieldDoom;

    public void ShowShield()
    {
        shieldDoom.SetActive(true);
    }
    public void HideShield()
    {
        shieldDoom.SetActive(false);
    }

    public void SetTeamColor(Material color)
    {
        foreach (Renderer renderer in renderMaterials)
        {
            renderer.material = color;
        }
    }
}
