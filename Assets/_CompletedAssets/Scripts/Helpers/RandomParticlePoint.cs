using UnityEngine;
using System.Collections;

public class RandomParticlePoint : MonoBehaviour 
{
    [Range(0f, 1f)]
    public float normalizedTime;


    void OnValidate()
    {
        particleSystem.Simulate (normalizedTime, true, true);
    }
}
