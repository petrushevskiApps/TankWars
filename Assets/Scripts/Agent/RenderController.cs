using System.Collections.Generic;
using UnityEngine;

public class RenderController : MonoBehaviour
{
    [SerializeField] private List<Renderer> renderMaterials;

    public void SetTeamColor(Material color)
    {
        foreach(Renderer renderer in renderMaterials)
        {
            renderer.material = color;
        }
    }
}