using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : EnemyBullet
{

    private void Start()
    {
        damage = StatusManager.Instance.data.Boss_Dmg;
    }
    protected override void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
    }
}
