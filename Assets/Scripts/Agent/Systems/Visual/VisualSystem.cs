using System.Collections.Generic;
using UnityEngine;

public class VisualSystem : MonoBehaviour
{
    [SerializeField] private ParticlesController particles;
    [SerializeField] private RenderController renderer;
    [SerializeField] private AgentUIController agentUI;
    [SerializeField] private GameObject cameraTracker;
    [SerializeField] private GameObject destroyedAgentPrefab;

    public ParticlesController Particles { get => particles; }
    public RenderController Renderer { get => renderer; }
    public AgentUIController AgentUI { get => agentUI; }
    public Transform Tracker { get => cameraTracker.transform; }

    private Agent agent;

    public void Initialize(Agent agent, Material teamColor)
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
        agent.Navigation.OnAgentIdling.AddListener(Particles.StopDrivingParticles);
        agent.Navigation.OnAgentMoving.AddListener(Particles.PlayDrivingParticles);
    }
    private void UnregisterListeners()
    {
        agent.Navigation.OnAgentIdling.RemoveListener(Particles.StopDrivingParticles);
        agent.Navigation.OnAgentMoving.RemoveListener(Particles.PlayDrivingParticles);
    }

    public void InstantiateDestroyed()
    {
        Vector3 position = agent.gameObject.transform.position;
        Quaternion rotation = agent.gameObject.transform.rotation;
        Transform parent = World.Instance.destroyedAgents;

        GameObject destroyedAgent = Instantiate(destroyedAgentPrefab, position, rotation, parent);
        destroyedAgent.name = destroyedAgent.name.Replace("(Clone)", "( " + agent.AgentName + " ) ");
        DropTracker(destroyedAgent.transform);
        destroyedAgent.SetActive(true);
    }

    // On Agent death move tracker
    // to new parent ( destroyed agent )
    private void DropTracker(Transform parent)
    {
        cameraTracker.transform.parent = parent;
    }
}