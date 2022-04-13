using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    public float damage;
    Collider col;

    void Start()
    {
        SoundManager.Instance.PlaySoundSurround("Explosion", false, transform.position);
        Destroy(col, 1f);
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.GetComponentInParent<Entity>()?.OnDamage(damage);
        }
    }
}
