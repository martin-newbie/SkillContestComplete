using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Germ : Entity
{

    [Header("Germ")]
    public GameObject[] BodyParts;
    public GameObject Bullet;
    public Transform WeaponPos;
    public GameObject Explosion;
    public float delay;
    float curDelay;

    protected override void DamageEffect()
    {
        BodyParts.ToList().ForEach(item =>
        {
            item.transform.SetParent(null);
            item.tag = "Untagged";
            Rigidbody rb = item.AddComponent<Rigidbody>();
            rb.AddForce(Random.insideUnitSphere * Random.Range(10f, 15f));
            rb.AddTorque(Random.insideUnitSphere * Random.Range(10f, 15f));
            Destroy(item, 5f);
        });

        Instantiate(Explosion, transform.position, Quaternion.identity);

        Destroy(gameObject, 5f);
        gameObject.SetActive(false);
    }

    protected override void Start()
    {
        maxHp = StatusManager.Instance.data.G_Hp;
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if(curDelay >= delay)
        {
            Instantiate(Bullet, WeaponPos.position, WeaponPos.rotation * Quaternion.Euler(0, 0, 180));
            curDelay = 0f;
        }

        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);

        curDelay += Time.deltaTime;
    }
}
