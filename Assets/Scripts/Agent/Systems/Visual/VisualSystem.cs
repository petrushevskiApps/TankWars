using System.Collections.Generic;
using UnityEngine;

public class VisualSystem : MonoBehaviour
{
    [SerializeField] private ParticlesController particles;
    [SerializeField] private RenderController renderer;
    [SerializeField] private AgentUIController agentUI;
    [SerializeField] private GameObject cameraTracker;

    public ParticlesController Particles { get => particles; }
    public RenderController Renderer { get => renderer; }
    public AgentUIController AgentUI { get => agentUI; }
    public Transform Tracker { get => cameraTracker.transform; }

    private Agent agent;

    public void Setup(Agent agent, Material teamColor)
    {
        this.agent = agent;
        agentUI.Setup(agent);
        renderer.SetTeamColor(teamColor);

        RegisterListeners();
    }
    private void OnDestroy()
    {
        UnregisterListeners();
    }

    private void RegisterListeners()
    {
        agent.OnAgentDeath.AddListener(Particles.ShowDeathParticles);
    }
    private void UnregisterListeners()
    {
        agent.OnAgentDeath.RemoveListener(Particles.ShowDeathParticles);
    }
}