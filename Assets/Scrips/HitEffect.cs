using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem hitEffect = null;

    void Start()
    {
        hitEffect.Stop();
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Enemy")
        {
            hitEffect.Play();
        }
    }
}