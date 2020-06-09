using System;
using UnityEngine;

namespace Complete
{
    public class Shell : MonoBehaviour
    {
        public LayerMask m_TankMask;                        // Used to filter what the explosion affects, this should be set to "Players".
        public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.
        public AudioSource m_ExplosionAudio;                // Reference to the audio that will play on explosion.
        public float m_MaxDamage = 100f;                    // The amount of damage done if the explosion is centred on a tank.
        public float m_ExplosionForce = 1000f;              // The amount of force added to a tank at the centre of the explosion.
        public float m_MaxLifeTime = 2f;                    // The time in seconds before the shell is removed.
        public float m_ExplosionRadius = 5f;                // The maximum distance away from the explosion tanks can be and are still affected.

        private Agent owner;
        [SerializeField] private string agentName;
        [SerializeField] private Team teamID;


        private void Start ()
        {
            // If it isn't destroyed by then, destroy the shell after it's lifetime.
            Destroy (gameObject, m_MaxLifeTime);
        }

        public void SetOwner(Agent owner)
        {
            this.owner = owner;
            agentName = owner.name;
        }
        public Agent GetOwner()
        {
            return owner;
        }
        public string GetOwnerName()
        {
            return agentName;
        }

        private void OnTriggerEnter (Collider other)
        {
            // If tigger detects its owner or missile sensor - Abort trigger
            if (IsOwner(other) || other.gameObject.layer == LayerMask.NameToLayer("RadarSensor")) return;
            
			// Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
            Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_TankMask);

            // Go through all the colliders...
            for (int i = 0; i < colliders.Length; i++)
            {
                // ... and find their rigidbody.
                Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody> ();

                // If they don't have a rigidbody, go on to the next collider.
                if (!targetRigidbody) continue;

                // Find the TankHealth script associated with the rigidbody.
                Agent agent = targetRigidbody.GetComponent<Agent>();

                if(agent.Team.ID != owner.Team.ID)
                {
                    // Calculate the amount of damage the target should take based on it's distance from the shell.
                    float damage = CalculateDamage(targetRigidbody.position);

                    
                    // Deal this damage to the tank.
                    agent.TakeDamage(damage, owner);
                }
            }

            // Unparent the particles from the shell.
            m_ExplosionParticles.transform.parent = World.Instance.shellsParent;

            // Play the particle system.
            m_ExplosionParticles.Play();

            // Play the explosion sound effect.
            m_ExplosionAudio.Play();

            // Once the particles have finished, destroy the gameobject they are on.
            ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;
            Destroy (m_ExplosionParticles.gameObject, mainModule.duration);

            // Destroy the shell.
            Destroy (gameObject);
        }

        private bool IsOwner(Collider other)
        {
            Rigidbody body = other.attachedRigidbody;
            
            if(body!=null)
            {
                string ownerName = body.gameObject.name;
                return agentName.Equals(ownerName);
            }
            else
            {
                return false;
            }
        }

        private float CalculateDamage (Vector3 targetPosition)
        {
            // Create a vector from the shell to the target.
            Vector3 explosionToTarget = targetPosition - transform.position;

            // Calculate the distance from the shell to the target.
            float explosionDistance = explosionToTarget.magnitude;

            // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
            float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

            // Calculate damage as this proportion of the maximum possible damage.
            float damage = relativeDistance * m_MaxDamage;

            // Make sure that the minimum damage is always 0.
            damage = Mathf.Max (0f, damage);

            return damage;
        }
    }
}