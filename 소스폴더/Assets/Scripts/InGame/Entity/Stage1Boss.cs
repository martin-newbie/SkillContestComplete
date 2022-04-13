using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Boss : Entity
{

    [Header("Boss")]
    public Transform Eye;
    public Transform EyeOut;
    public GameObject Bullet;
    public float EyeDelay;
    public bool Mad;
    public GameObject MadObject;
    bool attackAble;
    Vector3 eyePos;
    Vector3 outPos;
    Player player;
    Coroutine attackCoroutine;

    protected override void DamageEffect()
    {

    }

    IEnumerator EyeMove()
    {
        while (isAlive)
        {
            Vector2 randPos = Random.insideUnitCircle * 0.3f;
            eyePos.x = randPos.x;
            eyePos.y = -1f;
            eyePos.z = randPos.y;
            yield return new WaitForSeconds(EyeDelay);
            Attack();
        }
    }

    void Attack()
    {
        int count = Mad ? 5 : 3;
        float offset = 120 / count;
        int spawnOffset = Mad ? 0 : 1;

        Vector3 dir = player.transform.position - Eye.transform.position;
        dir.Normalize();
        Quaternion rot = Quaternion.LookRotation(dir);

        for (float i = Mad ? -2 * offset : -offset; i < 120 - (offset * spawnOffset); i += offset)
        {
            Instantiate(Bullet, Eye.position, Quaternion.Euler(0, i, 0) * rot);
        }
    }

    public override void OnDamage(float damage)
    {
        if (attackAble)
        {
            hp -= damage;

            if (hp <= 0 && !Mad)
            {
                StopCoroutine(attackCoroutine);
                maxHp *= 1.5f;
                hp = maxHp;
                Mad = true;
                EyeDelay = 1f;
                StartCoroutine(MadCinematic());
            }
            else if (hp <= 0 && Mad && isAlive)
            {
                isAlive = false;
                Score();
                DamageEffect();
                StartCoroutine(BossOutro());
            }
        }
    }

    IEnumerator MadCinematic()
    {
        attackAble = false;
        GameManager.Instance.CinematicStart();
        yield return new WaitForSeconds(1f);
        player.transform.position = Vector3.zero;

        Vector3 originPos = transform.position;

        float timer = 5f;
        float amount = 0.1f;
        while (timer > 0f)
        {
            Vector3 randPos = Random.insideUnitSphere * amount;
            transform.position = randPos + originPos;

            if (amount < 3f) amount += Time.deltaTime;
            timer -= Time.deltaTime;
            yield return null;
        }
        transform.position = originPos;

        outPos = new Vector3(0, 0.1f, 0);
        eyePos = new Vector3(0, -1f, -0.3f);

        yield return new WaitForSeconds(1f);

        outPos = Vector3.zero;
        eyePos = new Vector3(0, -1f, 0);
        MadObject.SetActive(true);
        GameManager.Instance.CinematicEnd();
        yield return new WaitForSeconds(1f);
        attackAble = true;
        attackCoroutine = StartCoroutine(EyeMove());
    }

    protected override void Start()
    {
        maxHp = StatusManager.Instance.data.Boss_1_Hp;
        base.Start();
        player = GameManager.Instance.player;
        StartCoroutine(BossIntro());
    }

    IEnumerator BossOutro()
    {
        GameManager.Instance.StageEnd();
        while (transform.position.y >= -50f)
        {
            transform.Translate(Vector3.down * Time.deltaTime * 2f);
            yield return null;
        }
    }

    IEnumerator BossIntro()
    {
        attackAble = false;
        GameManager.Instance.CinematicStart();
        transform.position = new Vector3(0, -25f, 50f);
        while (transform.position.y <= 0f)
        {
            transform.Translate(Vector3.up * Time.deltaTime * 2f);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        player.transform.position = Vector3.zero;

        float waitTime = 1f;
        int count = 30;

        while (count > 0)
        {
            SoundManager.Instance.PlaySound("ding");
            Vector3 randPos = Random.insideUnitCircle * 0.35f;
            eyePos.x = randPos.x;
            eyePos.y = -1f;
            eyePos.z = randPos.y;
            count--;
            yield return new WaitForSeconds(waitTime);
            if (waitTime > 0.05f) waitTime -= 0.05f;
        }

        outPos = new Vector3(0, 0.1f, 0);
        eyePos = new Vector3(0, -1f, -0.3f);

        yield return new WaitForSeconds(1f);

        outPos = Vector3.zero;
        eyePos = new Vector3(0, -1f, 0);

        yield return new WaitForSeconds(1f);
        GameManager.Instance.CinematicEnd();
        yield return new WaitForSeconds(1f);
        attackAble = true;
        attackCoroutine = StartCoroutine(EyeMove());
    }

    protected override void Update()
    {
        base.Update();

        Eye.localPosition = Vector3.Lerp(Eye.localPosition, eyePos, Time.deltaTime * 15f);
        EyeOut.localPosition = Vector3.Lerp(EyeOut.localPosition, outPos, Time.deltaTime * 15f);
        UIManager.Instance.SetBossMonsterHp(hp / maxHp, maxHp);
    }
}
