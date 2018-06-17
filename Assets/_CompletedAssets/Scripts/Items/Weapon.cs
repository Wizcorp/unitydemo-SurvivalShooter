using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject
{
    public class Weapon : MonoBehaviour
    {

        public string wepName = "GenericWeapon";

        public float useDelay = .5f;
        protected float deltaTime = 0;
        [SerializeField] private int currentAmmo = 0;
        public int maxAmmo = 99;

        public WeaponEffect[] wepHostileEffectArr;

        private void Update()
        {
            deltaTime += Time.deltaTime;
        }

        //on derived classes, this should be overridden in order to implement different weapon types
        protected virtual void ShootBehavior()
        {
            print("Shot the gun");
            for (int i = 0; i < wepHostileEffectArr.Length; i++)
            {
                wepHostileEffectArr[i].CauseEffect(gameObject);
            }
        }

        public void UseWeapon()
        {
            if (deltaTime > useDelay)
            {
                if (currentAmmo > 0)
                {
                    ShootBehavior();
                    currentAmmo--;
                    deltaTime = 0;
                }
                else { print("current weapon is out of ammo"); }
            }
        }

        /////////////////////////////////// inventory management helpers //////////////////////////////////    
        //mostly functions pertaining to current ammo, as it shouldn't be tampered with outside this script

        public bool AmmoNotFull()
        {
            if (currentAmmo < maxAmmo) { return true; }
            return false;
        }

        public void AddAmmo(int ammoToAdd)
        {
            print("Adding " + ammoToAdd + " to current ammo of " + currentAmmo);
            currentAmmo += ammoToAdd;
            if (currentAmmo > maxAmmo) { currentAmmo = maxAmmo; }
        }

        public void ResetAmmo()
        {
            currentAmmo = 0;
        }

        public int ReturnCurrentAmmo()
        {
            return currentAmmo;
        }
    }
}
