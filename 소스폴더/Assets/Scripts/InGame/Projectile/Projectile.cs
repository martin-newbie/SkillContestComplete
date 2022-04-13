using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Projectile : MonoBehaviour
{
    WeaponState thisState;
    Player player;
    public float moveSpeed;
    public float damage;

    protected virtual void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * moveSpeed);

        if (transform.position.z >= 200f)
        {
            Push();
        }
    }
    public void Init(WeaponState _state, Player _player)
    {
        thisState = _state;
        player = _player;
    }

    protected virtual void DestroyEffect()
    {

    }

    protected void Push()
    {
        DestroyEffect();
        player.Push(thisState, this);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponentInParent<Entity>().OnDamage(damage);
            Push();
        }
        else if (!other.CompareTag("EnemyBullet") && !other.CompareTag("ETC"))
            Push();
    }
}
