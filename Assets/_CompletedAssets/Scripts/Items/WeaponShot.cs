using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject
{
    public class WeaponShot : Weapon
    {

        //additional variables needed for shot behavior
        public float range = 100f;
        public int pelletsPerShot = 1;
        public float aimVariance = 0;

        //variables for audio/visual effects
        Ray shootRay;                                   // A ray from the gun end forwards.
        RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
        int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
        ParticleSystem gunParticles;                    // Reference to the particle system.
        LineRenderer gunLine;                           // Reference to the line renderer.
        AudioSource gunAudio;                           // Reference to the audio source.
        Light gunLight;                                 // Reference to the light component.
        float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.

        void Awake()
        {
            // Create a layer mask for the Shootable layer.
            shootableMask = LayerMask.GetMask("Shootable");

            // Set up the references.
            gunParticles = GetComponentInChildren<ParticleSystem>();
            gunLine = GetComponentInChildren<LineRenderer>();
            gunAudio = GetComponentInChildren<AudioSource>();
            gunLight = GetComponentInChildren<Light>();
        }

        private void Update()
        {
            deltaTime += Time.deltaTime;

            // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
            if (deltaTime >= useDelay * effectsDisplayTime)
            {
                // ... disable the effects.
                DisableEffects();
            }
        }

        public void DisableEffects()
        {
            // Disable the line renderer and the light.
            gunLine.enabled = false;
            gunLight.enabled = false;
        }

        protected override void ShootBehavior()
        {
            // Play the gun shot audioclip.
            gunAudio.Play();

            // Enable the light.
            gunLight.enabled = true;

            // Stop the particles from playing if they were, then start the particles.
            gunParticles.Stop();
            gunParticles.Play();

            // Enable the line renderer and set it's first position to be the end of the gun.
            gunLine.enabled = true;
            gunLine.SetPosition(0, gunLine.transform.position);

            // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
            shootRay.origin = gunLine.transform.position;
            shootRay.direction = gunLine.transform.forward;

            // Perform the raycast against gameobjects on the shootable layer and if it hits something...
            if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
            {
                for (int i = 0; i < wepHostileEffectArr.Length; i++)
                {
                    wepHostileEffectArr[i].CauseEffect(shootHit.collider.gameObject);
                }

                //play particle effect on any enemies hit
                //NOTE: this could/should at some point be replaced to play particles on ANYTHING hit
                EnemyHealth hitEnemy = shootHit.collider.GetComponent<EnemyHealth>();
                if (hitEnemy) { hitEnemy.PlayDamageEffect(shootHit.point); }

                // Set the second position of the line renderer to the point the raycast hit.
                gunLine.SetPosition(1, shootHit.point);
            }
            // If the raycast didn't hit anything on the shootable layer...
            else
            {
                // ... set the second position of the line renderer to the fullest extent of the gun's range.
                gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
            }
        }        
    }
}
