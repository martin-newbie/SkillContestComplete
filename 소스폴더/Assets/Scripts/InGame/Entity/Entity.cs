using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public float maxHp;
    public float moveSpeed;
    public float damage;
    public bool isMonster = true;
    [SerializeField] protected float hp;
    public bool isAlive = true;

    protected TextMesh scoreObj;

    protected virtual void Start()
    {
        scoreObj = GameManager.Instance.ScoreObject;
        hp = maxHp;
    }

    protected virtual void Update()
    {
        if(transform.position.z < 0f && isMonster && isAlive)
        {
            DamageEffect();
            isAlive = false;
            GameManager.Instance.Pp += maxHp / 2;
        }

        hp = Mathf.Clamp(hp, 0, maxHp);
    }

    public virtual void OnDamage(float damage)
    {
        hp -= damage;
        if(hp <= 0 && isAlive)
        {
            DamageEffect();
            isAlive = false;
            Score();
        }
    }

    protected void Score()
    {
        TextMesh temp = Instantiate(scoreObj, transform.position, Quaternion.identity);
        temp.text = "+" + string.Format("{0:0}", maxHp * 100f);

        GameManager.Instance.Score += maxHp * 100f;
        Destroy(temp.gameObject, 2f);
    }

    protected abstract void DamageEffect();
}
