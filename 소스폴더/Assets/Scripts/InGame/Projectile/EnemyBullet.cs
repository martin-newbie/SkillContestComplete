using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float moveSpeed;
    public float damage;
    public bool type = true;
    private void Start()
    {
        if (type) damage = StatusManager.Instance.data.E_Dmg_1;
        else damage = StatusManager.Instance.data.E_Dmg_2;


        Destroy(gameObject, 15f);
    }

    protected virtual void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * moveSpeed);
    }
}
