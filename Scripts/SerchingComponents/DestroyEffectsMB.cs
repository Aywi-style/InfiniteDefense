using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffectsMB : MonoBehaviour
{
    [SerializeField] private ParticleSystem DestroyExplosion;
    [SerializeField] private ParticleSystem DestroyFire;

    public ParticleSystem GetDestroyExplosion()
    {
        return DestroyExplosion;
    }

    public ParticleSystem GetDestroyFire()
    {
        return DestroyFire;
    }
}
