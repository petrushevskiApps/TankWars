using Complete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CommunicationSystem : MonoBehaviour
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
        foreach(AIAgent agent in agent.GetTeamMembers())
        {
            // No need to listen your own broadcast
            if(!agent.Equals(this.agent))
            {
                agent.GetCommunication().NeedHealth.AddListener(SendHealthLocations);
                agent.GetCommunication().NeedAmmo.AddListener(SendAmmoLocations);
                agent.GetCommunication().UnderAttack.AddListener(audioSensor.CallForHelp);
            }
        }
    }

    private void UnregisterToBroadcast()
    {
        foreach (AIAgent agent in agent.GetTeamMembers())
        {
            // No need to listen your own broadcast
            if (!agent.Equals(this.agent))
            {
                agent.GetCommunication().NeedHealth.RemoveListener(SendHealthLocations);
                agent.GetCommunication().NeedAmmo.RemoveListener(SendAmmoLocations);
                agent.GetCommunication().UnderAttack.AddListener(audioSensor.CallForHelp);
            }
        }
    }

    private void SendAmmoLocations(Action<List<GameObject>> receiver)
    {
        List<GameObject> detectedList = agent.GetMemory().AmmoPacks.GetDetectedList();

        if(detectedList.Count > 0)
        {
            receiver.Invoke(detectedList);
        }
    }

    private void SendHealthLocations(Action<List<GameObject>> receiver)
    {
        List<GameObject> detectedList = agent.GetMemory().HealthPacks.GetDetectedList();

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
        UnderAttack.Invoke(shell.GetComponent<Shell>().GetOwner());
    }


    public class AudioEvent : UnityEvent<Action<List<GameObject>>>
    {

    }

    public class UnderAttackEvent : UnityEvent<GameObject>
    {

    }
}
