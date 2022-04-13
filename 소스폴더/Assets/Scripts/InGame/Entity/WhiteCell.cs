using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteCell : Entity
{
    [Header("White Cell")]
    public GameObject Effect;
    Item thisItem;

    protected override void DamageEffect()
    {
        GameObject temp = Instantiate(Effect, transform.position, Quaternion.identity);
        Destroy(temp, 3f);
        thisItem.SetItemEffect();

        Destroy(gameObject, 5f);
        gameObject.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
        thisItem = GetComponent<Item>();
    }

    protected override void Update()
    {
        base.Update();
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
    }
}
