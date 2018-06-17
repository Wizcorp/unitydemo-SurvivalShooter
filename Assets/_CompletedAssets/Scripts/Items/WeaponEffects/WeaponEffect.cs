using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject
{
    public class WeaponEffect : MonoBehaviour
    {

        public virtual void CauseEffect(GameObject target)
        {
            Debug.Log("Effect caused on " + target.ToString());
        }

    }
}
