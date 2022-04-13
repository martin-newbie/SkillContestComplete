using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bacteria : Entity
{

    [Header("Bacteria")]
    public ParticleSystem destroyEffect;

    protected override void Start()
    {
        maxHp = StatusManager.Instance.data.B_Hp;
        base.Start();
    }
    protected override void DamageEffect()
    {
        ParticleSystem temp = Instantiate(destroyEffect, transform.position, Quaternion.identity);
        temp.Play();
        Destroy(temp.gameObject, 5f);

        Destroy(gameObject, 5f);
        gameObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
        transform.Translate(Vector3.back * Time.deltaTime * moveSpeed);
    }
}
