using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [SerializeField] GameObject hitEffect = null;
    
    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Weapon")
        {
            EffectGeneration();
        }
    }

    void EffectGeneration()
    {
        GameObject effect = Instantiate(hitEffect, new Vector3(gameObject.transform.position.x,
            transform.position.y * 1.5f, gameObject.transform.position.z), Quaternion.identity);
        //effect.transform.position = gameObject.transform.position;
    }
}