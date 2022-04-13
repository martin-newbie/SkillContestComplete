using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Virus : Entity
{
    [Header("Virus")]
    public GameObject[] Guns = new GameObject[2];
    public Transform[] GunPoses = new Transform[2];
    public GameObject Bullet;
    public float Delay;
    public ParticleSystem destroyEffect;
    float curDelay;
    Player player;

    protected override void DamageEffect()
    {

        Guns.ToList().ForEach(item =>
        {
            item.transform.SetParent(null);
            Rigidbody rb = item.AddComponent<Rigidbody>();
            rb.AddForce(Random.insideUnitSphere * 15f, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 15f);
            Destroy(item.gameObject, 5f);
        });
        Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Destroy(gameObject, 5f);
        gameObject.SetActive(false);

    }

    protected override void Start()
    {
        maxHp = StatusManager.Instance.data.V_Hp;
        base.Start();
        player = GameManager.Instance.player;
    }

    protected override void Update()
    {
        base.Update();

        Guns.ToList().ForEach(item => item.transform.LookAt(player.transform.position));

        if(curDelay >= Delay)
        {
            GunPoses.ToList().ForEach(item => Instantiate(Bullet, item.position, item.rotation));
            curDelay = 0f;
        }
        curDelay += Time.deltaTime;

        transform.Translate(Vector3.back * Time.deltaTime * moveSpeed);
    }
}
