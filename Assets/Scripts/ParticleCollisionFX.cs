using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForcefieldDemo
{
    // requires a primary Particle System to drive the collision particles
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleCollisionFX : MonoBehaviour
    {
        // secondary particles spawn on impact
        [SerializeField] private ParticleSystem collisionFXPrefab;

        // destroy the particles after n seconds
        [SerializeField] private float lifeTime = 1.5f;


        // the primary ParticleSystem used to generate the collision
        private ParticleSystem primaryFX;


        private readonly List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

        private void Awake()
        {
            primaryFX = GetComponent<ParticleSystem>();
        }

        private void OnParticleCollision(GameObject other)
        {
            if (collisionFXPrefab == null)
            {
                Debug.Log("PARTICLE COLLISION FX OnParticleCollsiion: missing Collision FX Prefab!");
                return;
            }

            // loop through all Collision Events
            int count = primaryFX.GetCollisionEvents(other, collisionEvents);
            int i = 0;

            while (i < count)
            {
                // instantiate collision particles at each intersection
                var collisionFX = Instantiate(collisionFXPrefab, collisionEvents[i].intersection, Quaternion.identity);

                // orient to face normal
                collisionFX.transform.rotation = Quaternion.LookRotation(collisionEvents[i].normal);

                // if the other object has a ForcefieldImpact component, activate the forcefield ripple
                ForcefieldImpact forcefieldImpact = other.GetComponent<ForcefieldImpact>();

                if (forcefieldImpact != null)
                {
                    forcefieldImpact.ApplyImpact(collisionEvents[i].intersection, collisionEvents[i].normal);
                }

                // destroy the collision effect after delay and increment
                Destroy(collisionFX.gameObject, lifeTime);

                i++;
            }
        }
    }
}