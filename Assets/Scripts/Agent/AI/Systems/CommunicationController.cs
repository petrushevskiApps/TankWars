using Complete;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CommunicationController : MonoBehaviour
{
    [HideInInspector]
    public AudioEvent NeedHealth = new AudioEvent();

    [HideInInspector]
    public AudioEvent NeedAmmo = new AudioEvent();

    [HideInInspector]
    public UnderAttackEvent UnderAttack = new UnderAttackEvent();


    [SerializeField] private AudioSensor audioSensor;

    private AIAgent agent;

    public void Initialize(AIAgent agent)
    {
        this.agent = agent;
        agent.GetPerceptor().OnUnderAttack.AddListener(BroadcastUnderAttack);
        RegisterToBroadcasts();
    }

    private void OnDestroy()
    {
        agent.GetPerceptor().OnUnderAttack.RemoveListener(BroadcastUnderAttack);
        UnregisterToBroadcast();
    }

    private void RegisterToBroadcasts()
    {
        foreach(Agent agent in agent.Team.Members)
        {
            if(agent.GetType() == typeof(AIAgent))
            {
                AIAgent aiAgent = (AIAgent)agent;

                // No need to listen your own broadcast
                if (!aiAgent.Equals(this.agent))
                {
                    aiAgent.Communication.NeedHealth.AddListener(SendHealthLocations);
                    aiAgent.Communication.NeedAmmo.AddListener(SendAmmoLocations);
                    aiAgent.Communication.UnderAttack.AddListener(audioSensor.CallForHelp);
                }
            }
        }
    }

    private void UnregisterToBroadcast()
    {
        foreach (Agent agent in agent.Team.Members)
        {
            if (agent.GetType() == typeof(AIAgent))
            {
                AIAgent aiAgent = (AIAgent)agent;

                // No need to listen your own broadcast
                if (!aiAgent.Equals(this.agent))
                {
                    aiAgent.Communication.NeedHealth.RemoveListener(SendHealthLocations);
                    aiAgent.Communication.NeedAmmo.RemoveListener(SendAmmoLocations);
                    aiAgent.Communication.UnderAttack.RemoveListener(audioSensor.CallForHelp);
                }
            }
        }
    }

    private void SendAmmoLocations(Action<List<GameObject>> receiver)
    {
        List<GameObject> detectedList = agent.Memory.AmmoPacks.GetDetectedList();

        if(detectedList.Count > 0)
        {
            receiver.Invoke(detectedList);
        }
    }

    private void SendHealthLocations(Action<List<GameObject>> receiver)
    {
        List<GameObject> detectedList = agent.Memory.HealthPacks.GetDetectedList();

        if (detectedList.Count > 0)
        {
            receiver.Invoke(detectedList);
        }
    }

    public void BroadcastNeedHealth()
    {
        NeedHealth.Invoke((detected) => audioSensor.ReceiveLocations(detected));
    }
    public void BroadcastNeedAmmo()
    {
        NeedAmmo.Invoke((detected) => audioSensor.ReceiveLocations(detected));
    }

    public void BroadcastUnderAttack(GameObject shell)
    {
        UnderAttack.Invoke(shell.GetComponent<Shell>().GetOwner().gameObject);
    }


    public class AudioEvent : UnityEvent<Action<List<GameObject>>> { }

    public class UnderAttackEvent : UnityEvent<GameObject> { }
}
