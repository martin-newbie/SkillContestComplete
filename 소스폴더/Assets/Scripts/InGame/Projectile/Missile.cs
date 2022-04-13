using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Projectile
{
    public GameObject Explosion;
    protected override void DestroyEffect()
    {
        base.DestroyEffect();
        Instantiate(Explosion, transform.position, Quaternion.identity);
    }
}
