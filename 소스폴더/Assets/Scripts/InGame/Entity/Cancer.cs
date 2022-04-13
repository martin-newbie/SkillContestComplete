using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cancer : Entity
{
    [Header("Cancer")]
    public GameObject[] parts;
    public GameObject Bullet;
    public Transform WeaponPos;
    public ParticleSystem DestroyEffect;
    public float delay;
    float curDelay;
    Player player;

    protected override void Start()
    {
        maxHp = StatusManager.Instance.data.C_Hp;
        base.Start();
        player = GameManager.Instance.player;
    }

    protected override void DamageEffect()
    {
        Instantiate(DestroyEffect, transform.position, Quaternion.identity);

        parts.ToList().ForEach(item =>
        {
            item.transform.SetParent(null);
            item.tag = "Untagged";
            Rigidbody rb = item.AddComponent<Rigidbody>();
            rb.AddForce(Random.insideUnitSphere * Random.Range(5f, 15f), ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * Random.Range(5f, 15f));
            Destroy(item.gameObject, 5f);
        });

        Destroy(gameObject, 5f);
        gameObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (curDelay >= delay)
        {
            ShootToPlayer();
            curDelay = 0f;
        }
        curDelay += Time.deltaTime;

        transform.Translate(Vector3.back * Time.deltaTime * moveSpeed);
    }

    void ShootToPlayer()
    {
        Vector3 dir = player.transform.position - WeaponPos.position;
        dir.Normalize();
        Quaternion rot = Quaternion.LookRotation(dir);

        Instantiate(Bullet, WeaponPos.position, rot * Quaternion.Euler(-90, 0, 0));
    }
}
