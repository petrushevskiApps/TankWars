using Complete;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SensorController : MonoBehaviour
{
    [SerializeField] private VisionSensor visionSensor = new VisionSensor();
    [SerializeField] private RadarSensor radarSensor = new RadarSensor();

    public AgentEvent OnFriendlyDetected = new AgentEvent();
    public AgentEvent OnFriendlyLost = new AgentEvent();

    public AgentEvent OnEnemyDetected = new AgentEvent();
    public AgentEvent OnEnemyLost = new AgentEvent();

    public PackageEvent OnAmmoPackDetected = new PackageEvent();
    public PackageEvent OnAmmoPackLost = new PackageEvent();

    public PackageEvent OnHealthPackDetected = new PackageEvent();
    public PackageEvent OnHealthPackLost = new PackageEvent();

    public HiddingSpot OnHidingSpotDetected = new HiddingSpot();
    public HiddingSpot OnHidingSpotLost = new HiddingSpot();

    public MissileEvent OnUnderAttack = new MissileEvent();

    private Color ENEMY_VISIBLE_RAY_COLOR = Color.red;
    private Color ENEMY_RADAR_RAY_COLOR = Color.gray;

    private Color FRENDLY_VISIBLE_RAY_COLOR = Color.cyan;
    private Color FRENDLY_RADAR_RAY_COLOR = Color.cyan;

    private Color HIDING_SPOT_RAY_COLOR = Color.blue;
    private Color PICKABLE_RAY_COLOR = Color.green;

    private Dictionary<string, AgentVisibilityStatus> enemiesVisibility = new Dictionary<string, AgentVisibilityStatus>();
    private Dictionary<string, AgentVisibilityStatus> friendsVisibility = new Dictionary<string, AgentVisibilityStatus>();

    private void Awake()
    {
        RegisterToSensors();
    }
    private void OnDestroy()
    {
        UnregisterToSensors();
    }

    private void RegisterToSensors()
    {
        visionSensor.OnDetected.AddListener(InSight);
        visionSensor.OnLost.AddListener(LostSight);

        radarSensor.OnDetected.AddListener(OnRadarDetected);
        radarSensor.OnLost.AddListener(OnRadarLost);

    }

    private void UnregisterToSensors()
    {
        visionSensor.OnDetected.RemoveListener(InSight);
        visionSensor.OnLost.RemoveListener(LostSight);

        radarSensor.OnDetected.RemoveListener(OnRadarDetected);
        radarSensor.OnLost.RemoveListener(OnRadarLost);

    }

    private void InSight(GameObject detected, bool isVisible)
    {
        if (detected.CompareTag("Tank"))
        {
            // If visible : Detected
            // If invisible : Lost
            AgentDetected(detected, isVisible, false);
        }
        else if(isVisible && detected.CompareTag("Pickable"))
        {
            // Pickables can be detected only if visible
            PickableDetected(detected);
        }
        
    }

    private void LostSight(GameObject lost)
    {
        if (lost.CompareTag("Tank"))
        {
            AgentDetected(lost, false, false);
        }
    }

    private void AgentDetected(GameObject detected, bool isVisible, bool isRadar)
    {
        Agent agent = detected.GetComponent<Agent>();

        if (agent == null) return;

        if (IsEnemy(agent.Team.ID))
        {
            if (isVisible)
            {
                DrawDebugRay(agent.gameObject, isRadar ? ENEMY_RADAR_RAY_COLOR : ENEMY_VISIBLE_RAY_COLOR);

                SetAgentVisibilityStatus(agent, enemiesVisibility, isVisible, isRadar);

                OnEnemyDetected.Invoke(agent.gameObject);
            }
            else
            {
                if (enemiesVisibility.ContainsKey(agent.name))
                {
                    SetAgentVisibilityStatus(agent, enemiesVisibility, isVisible, isRadar);

                    if (!enemiesVisibility[agent.name].IsVisible())
                    {
                        OnEnemyLost.Invoke(agent.gameObject);
                        enemiesVisibility.Remove(agent.name);
                    }
                }
            }
        }
        else
        {
            if (isVisible)
            {
                DrawDebugRay(detected, isRadar ? FRENDLY_RADAR_RAY_COLOR : FRENDLY_VISIBLE_RAY_COLOR);
                SetAgentVisibilityStatus( agent, friendsVisibility, isVisible, isRadar);
                OnFriendlyDetected.Invoke(detected);
            }
            else
            {
                if (friendsVisibility.ContainsKey(agent.name))
                {
                    SetAgentVisibilityStatus( agent, friendsVisibility, isVisible, isRadar);

                    if (!friendsVisibility[agent.name].IsVisible())
                    {
                        OnFriendlyLost.Invoke(detected);
                        friendsVisibility.Remove(agent.name);
                    }
                }
            }
        }
    }

    private void SetAgentVisibilityStatus(Agent agent, Dictionary<string, AgentVisibilityStatus> visibility, bool isVisible, bool isRadar)
    {
        if (!visibility.ContainsKey(agent.name))
        {
            visibility.Add(agent.name, new AgentVisibilityStatus());
        }

        if (isRadar)
        {
            // Set is agent visible by radar
            visibility[agent.name].radarVisible = isVisible;
        }
        else
        {
            // Set is agent visible by sight
            visibility[agent.name].visionVisible = isVisible;
        }
    }

    private void PickableDetected(GameObject detected)
    {
        Pickable pickable = detected.GetComponent<Pickable>();

        if (pickable.GetType().Equals(typeof(AmmoPack)))
        {
            DrawDebugRay(detected, PICKABLE_RAY_COLOR);
            OnAmmoPackDetected.Invoke(detected);
        }
        else if(pickable.GetType().Equals(typeof(HealthPack)))
        {
            DrawDebugRay(detected, PICKABLE_RAY_COLOR);
            OnHealthPackDetected.Invoke(detected);
        }
    }


    private void OnRadarDetected(GameObject detected, bool isVisible)
    {
        if (detected.CompareTag("HidingSpot"))
        {
            // Hiding spot can be detected if in sight range
            // both visible and invisible
            DrawDebugRay(detected, HIDING_SPOT_RAY_COLOR);
            OnHidingSpotDetected.Invoke(detected);
        }
        else if (detected.CompareTag("Missile"))
        {
            Shell missile = detected.GetComponent<Shell>();

            if (missile != null)
            {
                if (IsEnemy(missile.GetOwner().Team.ID))
                {
                    OnUnderAttack.Invoke(detected);
                }
            }
        }
        else if (detected.CompareTag("Tank"))
        {
            AgentDetected(detected, isVisible, true);
        }
    }

    private void OnRadarLost(GameObject lost)
    {
        if (lost.CompareTag("HidingSpot"))
        {
            OnHidingSpotLost.Invoke(lost);
        }
        else if (lost.CompareTag("Tank"))
        {
            AgentDetected(lost, false, true);
        }
    }

    private bool IsEnemy(int targetTeamID)
    {
        int ID = transform.parent.GetComponent<Agent>().Team.ID;
        return ID != targetTeamID;
    }

    private void DrawDebugRay(GameObject detected, Color rayColor)
    {
        Debug.DrawRay(transform.position, detected.transform.position - transform.position, rayColor);
    }

    public class AgentEvent : UnityEvent<GameObject> { }
    public class PackageEvent : UnityEvent<GameObject> { }
    public class HiddingSpot : UnityEvent<GameObject> { }
    public class MissileEvent : UnityEvent<GameObject> { }

    public class AgentVisibilityStatus
    {
        public bool visionVisible = false;
        public bool radarVisible = false;

        public bool IsVisible()
        {
            return visionVisible || radarVisible;
        }
    }
}
