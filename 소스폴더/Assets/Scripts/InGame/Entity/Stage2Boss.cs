using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stage2Boss : Entity
{

    [Header("Boss2")]
    public Transform Bodies;
    public Transform Eyes;
    public GameObject EyeProtections;
    public bool Mad;
    public GameObject madObj;
    public GameObject Bullet;
    public Entity Bacteria;
    Vector3 eyePoses;
    Player player;
    Coroutine attackCoroutine;
    bool attackAble = true;
    float EyeDelay = 2f;
    float rotX, rotY, rotZ;
    GameObject protect2;

    protected override void DamageEffect()
    {
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
                GameManager.Instance.StageEnd();
                isAlive = false;
                Score();
                StartCoroutine(BossOutro());
            }
        }
    }

    IEnumerator BossOutro()
    {
        attackAble = false;
        while (transform.position.y >= -50f)
        {
            transform.Translate(Vector3.down * Time.deltaTime * 2f);
            yield return null;
        }

    }

    protected override void Start()
    {
        maxHp = StatusManager.Instance.data.Boss_2_Hp;
        base.Start();
        player = GameManager.Instance.player;
        StartCoroutine(BossIntro());
    }

    protected override void Update()
    {
        base.Update();

        rotX += Time.deltaTime * moveSpeed;
        rotY += Time.deltaTime * moveSpeed;
        rotZ += Time.deltaTime * moveSpeed;

        Eyes.transform.localPosition = Vector3.Lerp(Eyes.transform.localPosition, eyePoses, Time.deltaTime * 15f);

        EyeProtections.transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);

        if (protect2 != null) protect2.transform.rotation = Quaternion.Euler(-rotX, -rotY, -rotZ);

        UIManager.Instance.SetBossMonsterHp(hp / maxHp, maxHp);
    }

    IEnumerator BossIntro()
    {
        attackAble = false;
        GameManager.Instance.CinematicStart();

        transform.position = new Vector3(0, -25f, 50f);

        while (transform.position.y < 0f)
        {
            transform.Translate(Vector3.up * Time.deltaTime * 2f);
            yield return null;
        }
        player.transform.position = Vector3.zero;
        EyeProtections.SetActive(true);
        Vector3 protScale = EyeProtections.transform.localScale;
        EyeProtections.transform.localScale = new Vector3(0, 0, 0);

        Vector3 offset = protScale / 3f;

        while (Vector3.Distance(protScale, EyeProtections.transform.localScale) > 0.001f)
        {
            EyeProtections.transform.localScale += offset * Time.deltaTime;
            yield return null;
        }

        EyeProtections.transform.localScale = protScale;

        yield return new WaitForSeconds(1f);
        GameManager.Instance.CinematicEnd();
        yield return new WaitForSeconds(1f);
        attackAble = true;
        attackCoroutine = StartCoroutine(AttackLogic());
    }

    IEnumerator AttackLogic()
    {
        while (attackAble)
        {
            int rand = Random.Range(0, 2);

            Vector3 randPos1 = Random.insideUnitCircle * 0.3f;

            eyePoses.x = randPos1.x;
            eyePoses.y = -1f;
            eyePoses.z = randPos1.y;

            if (rand == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    SpawnBullet(Eyes);
                    yield return new WaitForSeconds(0.2f);
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    SpawnEnemy(Eyes);
                    yield return new WaitForSeconds(0.5f);
                }
            }


            yield return new WaitForSeconds(EyeDelay);

        }
    }

    void SpawnEnemy(Transform pos)
    {
        Vector3 dir = player.transform.position - pos.position;
        dir.Normalize();
        Quaternion rot = Quaternion.LookRotation(dir);
        Entity temp = Instantiate(Bacteria, pos.position, rot);
        temp.moveSpeed *= -1f;
    }

    void SpawnBullet(Transform pos)
    {
        Vector3 dir = player.transform.position - pos.position;
        dir.Normalize();
        Quaternion rot = Quaternion.LookRotation(dir);
        Instantiate(Bullet, pos.position, rot);
    }

    IEnumerator MadCinematic()
    {
        attackAble = false;
        GameManager.Instance.CinematicStart();
        yield return new WaitForSeconds(1f);
        player.transform.position = Vector3.zero;

        float timer = 3f;
        Vector3 originPos = transform.position;
        protect2 = Instantiate(EyeProtections, EyeProtections.transform.parent);

        Vector3 originScale = protect2.transform.localScale;
        protect2.transform.localScale = Vector3.zero;
        Vector3 offset = originScale / timer;

        while (timer > 0f)
        {
            Vector3 randpos = Random.insideUnitSphere * 0.5f;
            protect2.transform.localScale += offset * Time.deltaTime;
            transform.position = originPos + randpos;

            timer -= Time.deltaTime;
            yield return null;
        }
        protect2.transform.localScale = originScale;
        transform.position = originPos;
        madObj.SetActive(true);


        yield return new WaitForSeconds(3f);


        GameManager.Instance.CinematicEnd();
        yield return new WaitForSeconds(1f);
        attackAble = true;
        attackCoroutine = StartCoroutine(AttackLogic());
        isAlive = true;
    }
}
