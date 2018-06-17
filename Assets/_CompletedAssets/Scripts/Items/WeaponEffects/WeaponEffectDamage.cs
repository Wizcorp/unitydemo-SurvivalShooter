using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject
{
    public class WeaponEffectDamage : WeaponEffect
    {

        public int damage = 20;

        public override void CauseEffect(GameObject target)
        {
            // Try and find an EnemyHealth script on the gameobject hit.
            EnemyHealth enemyHealth = target.GetComponent<EnemyHealth>();

            // If the EnemyHealth component exist...
            if (enemyHealth != null)
            {
                // ... the enemy should take damage.
                enemyHealth.TakeDamage(damage);
            }
        }
    }
}
