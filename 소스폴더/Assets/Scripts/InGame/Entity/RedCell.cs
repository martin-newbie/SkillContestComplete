using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCell : Entity
{

    public GameObject particle;

    protected override void DamageEffect()
    {
        Instantiate(particle, transform.position, Quaternion.identity);

        GameManager.Instance.Pp += 10f;

        Destroy(gameObject);
        gameObject.SetActive(false);
    }

    protected override void Update()
    {
        transform.Translate(Vector3.back * Time.deltaTime * moveSpeed);

        if (transform.position.z <= 0f) Destroy(gameObject);
    }
}
