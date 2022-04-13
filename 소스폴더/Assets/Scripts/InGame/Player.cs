using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum WeaponState
{
    MACHINEGUN,
    MISSILE,
    HOMING
}

public class Player : MonoBehaviour
{
    Coroutine bulletProof;

    [Header("Status")]
    public WeaponState curState;
    public int GunDelayLevel;
    public int GunDamageLevel;
    public int GunSpreadLevel;
    public LayerMask ableLayer;
    public float moveSpeed;
    public bool isBulletProof;
    public bool isActive = true;

    [Header("Objects")]
    public GameObject[] Guns = new GameObject[2];
    public GameObject[] GunsContainer = new GameObject[2];
    public Transform[] GunPoses = new Transform[2];
    public GameObject cam;
    public GameObject BulletProofObj;
    public GameObject[] meshes;
    public GameObject container;
    Vector3 originPos;

    [Header("Object Pool")]
    public Projectile Bullet;
    public Projectile Missile;
    public Projectile Homing;
    public Transform BulletPool;
    public Transform MissilePool;
    public Transform HomingPool;
    Stack<Projectile> BulletStack = new Stack<Projectile>();
    Stack<Projectile> MissileStack = new Stack<Projectile>();
    Stack<Projectile> HomingStack = new Stack<Projectile>();

    [Header("Gun Status")]
    public ParticleSystem[] Particles = new ParticleSystem[2];
    public ParticleSystem[] ExplosiveParticle = new ParticleSystem[2];
    public float gunDelay;
    public float gunSpread;
    public float gunDamage;
    float curDelay;

    [Header("Missile Status")]
    public int M_loaded;
    public int M_max;
    public int M_inventory;
    bool m_loading;

    [Header("Homing Status")]
    public int H_loaded;
    public int H_max;
    public int H_inventory;
    public LockOn lockonPrefab;
    [SerializeField] List<Transform> curLockedonEnemy = new List<Transform>();
    List<LockOn> curLockedonUI = new List<LockOn>();
    bool h_loading;
    bool curShoot;

    [Header("UI Objects")]
    public RectTransform AimPoint;
    public RectTransform canvasRect;

    float rotZ;

    void Start()
    {
        gunDamage = StatusManager.Instance.data.Dmg;
        moveSpeed = StatusManager.Instance.data.MS;

        originPos = cam.transform.localPosition;

        SoundManager.Instance.PlaySound("JetEngine", true);
        InitPool();
    }

    public void ItemBulletProof()
    {
        if (bulletProof != null) StopCoroutine(bulletProof);
        bulletProof = StartCoroutine(ItemBulletProofCoroutine());
    }

    IEnumerator ItemBulletProofCoroutine()
    {
        isBulletProof = true;
        yield return new WaitForSeconds(3f);
        isBulletProof = false;
    }

    void InitPool()
    {
        for (int i = 0; i < 500; i++)
        {
            Projectile temp = Instantiate(Bullet, BulletPool);
            temp.Init(WeaponState.MACHINEGUN, this);
            temp.gameObject.SetActive(false);
            BulletStack.Push(temp);
        }

        for (int i = 0; i < 50; i++)
        {
            Projectile temp = Instantiate(Missile, MissilePool);
            temp.Init(WeaponState.MISSILE, this);
            temp.gameObject.SetActive(false);
            MissileStack.Push(temp);
        }

        for (int i = 0; i < 20; i++)
        {
            Projectile temp = Instantiate(Homing, HomingPool);
            temp.Init(WeaponState.HOMING, this);
            temp.gameObject.SetActive(false);
            HomingStack.Push(temp);
        }
    }

    Projectile ShootProjectile(WeaponState state, Vector3 pos, Quaternion rot)
    {

        Projectile ret = null;

        switch (state)
        {
            case WeaponState.MACHINEGUN:
                ret = BulletStack.Pop();
                break;
            case WeaponState.MISSILE:
                ret = MissileStack.Pop();
                break;
            case WeaponState.HOMING:
                ret = HomingStack.Pop();
                break;
        }

        ret.transform.position = pos;
        ret.transform.rotation = rot;
        ret.gameObject.SetActive(true);
        return ret;
    }

    public void Push(WeaponState state, Projectile obj)
    {
        obj.gameObject.SetActive(false);
        switch (state)
        {
            case WeaponState.MACHINEGUN:
                BulletStack.Push(obj);
                break;
            case WeaponState.MISSILE:
                MissileStack.Push(obj);
                break;
            case WeaponState.HOMING:
                HomingStack.Push(obj);
                break;
        }
    }

    void Update()
    {
        Clamp();

        if (isActive)
        {
            MoveLogic();
            AttackLogic();
        }

        Cheat();
    }

    void Cheat()
    {
        if (Input.GetKeyDown(KeyCode.F1)) GameManager.Instance.NextStage();
        if (Input.GetKeyDown(KeyCode.F2)) GunDamageLevel++;
        if (Input.GetKeyDown(KeyCode.F3)) GunDelayLevel++;
        if (Input.GetKeyDown(KeyCode.F4)) GunSpreadLevel++;
        if (Input.GetKeyDown(KeyCode.F5)) isBulletProof = true;
        if (Input.GetKeyDown(KeyCode.F6)) isBulletProof = false;
        if (Input.GetKeyDown(KeyCode.F7)) GameManager.Instance.RemoveAllMonsters();
        if (Input.GetKeyDown(KeyCode.F8)) GameManager.Instance.Hp = 100f;
        if (Input.GetKeyDown(KeyCode.F9)) GameManager.Instance.Pp = 0f;
        if (Input.GetKeyDown(KeyCode.F10)) GameManager.Instance.InstantiateRedCell();
        if (Input.GetKeyDown(KeyCode.F11)) GameManager.Instance.InstantiateWhiteCell();
    }

    public void GameOver()
    {
        isActive = false;
        Debug.Log("gameover");
        ExplosiveParticle[0].Play();
        ExplosiveParticle[1].Play();

        meshes.ToList().ForEach(item =>
        {
            item.transform.SetParent(null);
            Rigidbody rb = item.AddComponent<Rigidbody>();
            rb.AddForce(Random.insideUnitSphere * Random.Range(-10f, 10f), ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * Random.Range(-10f, 10f));
        });
    }


    void Clamp()
    {
        GunDelayLevel = Mathf.Clamp(GunDelayLevel, 0, 5);
        GunDamageLevel = Mathf.Clamp(GunDamageLevel, 0, 5);
        GunSpreadLevel = Mathf.Clamp(GunSpreadLevel, 0, 5);
    }

    void MoveLogic()
    {
        //-80~80, 0~50
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        BulletProofObj.transform.Rotate(Vector3.forward * Time.deltaTime * 500f);

        transform.Translate(new Vector3(moveX, moveY, 0) * moveSpeed * Time.deltaTime);

        float clampX = Mathf.Clamp(transform.position.x, -80f, 80f);
        float clampY = Mathf.Clamp(transform.position.y, 0f, 50f);

        transform.position = new Vector2(clampX, clampY);

        rotZ = moveX * -30;
        Quaternion rot = Quaternion.Euler(0, 0, rotZ);
        container.transform.rotation = Quaternion.Lerp(container.transform.rotation, rot, Time.deltaTime * 5f);
    }

    void AttackLogic()
    {
        GunFollow();
        ChangeState();
        FollowAimpoint();
        HomingLoad();
        BulletProofObj.SetActive(isBulletProof);

        UIManager.Instance.SetCurWeapon((int)curState);
        UIManager.Instance.SetWeaponLevel(GunDamageLevel, GunDelayLevel, GunSpreadLevel);

        switch (curState)
        {
            case WeaponState.MACHINEGUN:
                MachingunAttack();
                UIManager.Instance.SetWeaponStatus(true);
                break;
            case WeaponState.MISSILE:
                MissileAttack();
                UIManager.Instance.SetWeaponStatus(false, M_loaded, M_max, M_inventory);
                break;
            case WeaponState.HOMING:
                HomingAttack();
                UIManager.Instance.SetWeaponStatus(false, H_loaded, H_max, H_inventory);
                break;
        }
    }

    void FollowAimpoint()
    {
        Vector2 screenPoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector2 viewPoint = new Vector2(
            (screenPoint.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
            (screenPoint.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)
            );

        AimPoint.anchoredPosition = viewPoint;
    }

    void HomingLoad()
    {
        if (H_loaded < H_max && !h_loading && H_inventory > 0 && curLockedonEnemy.Count <= 0)
        {
            StartCoroutine(HomingLoadCoroutine());
        }
    }

    IEnumerator HomingLoadCoroutine()
    {
        h_loading = true;
        float timer = 2f;

        while (timer > 0f)
        {
            UIManager.Instance.SetHLoading(timer / 2f);
            timer -= Time.deltaTime;
            yield return null;
        }

        H_loaded++;
        H_inventory--;

        h_loading = false;
    }

    void HomingAttack()
    {
        if (Input.GetMouseButtonDown(1) && H_loaded > 0)
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 300f, ableLayer))
            {
                Transform lockon = hit.transform;

                if (lockon.CompareTag("Enemy"))
                {
                    H_loaded--;
                    LockOn uiTmp = Instantiate(lockonPrefab, canvasRect.transform);
                    uiTmp.Init(canvasRect, lockon);

                    curLockedonEnemy.Add(lockon);
                    curLockedonUI.Add(uiTmp);
                    SoundManager.Instance.PlaySound("Lockon");
                }
            }

        }

        if (curLockedonEnemy.Count > 0 && Input.GetMouseButtonDown(0) && !curShoot)
        {
            StartCoroutine(HomingShoot());
        }
    }

    IEnumerator HomingShoot()
    {
        curShoot = true;
        for (int i = 0; i < curLockedonEnemy.Count; i++)
        {
            Projectile temp = ShootProjectile(WeaponState.HOMING, GunPoses[1].position, GunPoses[1].rotation);
            temp.GetComponent<HomingMissile>().HomingInit(GunPoses[1], curLockedonEnemy[i], 6, 4, curLockedonUI[i]);
            SoundManager.Instance.PlaySound("count");
            yield return new WaitForSeconds(0.2f);
        }

        curLockedonEnemy.Clear();
        curLockedonUI.Clear();
        curShoot = false;
    }

    void MissileAttack()
    {
        if (Input.GetMouseButtonDown(0) && !m_loading)
        {
            if (M_loaded > 0)
            {
                M_loaded--;
                SoundManager.Instance.PlaySound("Lockon");
                ShootProjectile(WeaponState.MISSILE, GunPoses[0].position, GunPoses[0].rotation * Quaternion.Euler(90, 0, 0));
            }
            else if (M_loaded <= 0)
            {
                StartCoroutine(MissileLoad());
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !m_loading && M_loaded < M_max)
        {
            StartCoroutine(MissileLoad());
        }
    }

    IEnumerator MissileLoad()
    {

        if (M_inventory > 0)
        {
            m_loading = true;
            float timer = 8f;
            while (timer > 0f)
            {
                UIManager.Instance.SetMLoading(timer / 8f);
                timer -= Time.deltaTime;
                yield return null;
            }

            if (M_inventory + M_loaded >= M_max)
            {
                M_inventory += M_loaded;
                M_inventory -= M_max;
                M_loaded = M_max;
            }
            else
            {
                M_loaded += M_inventory;
                M_inventory = 0;
            }

            m_loading = false;
        }
    }

    void MachingunAttack()
    {
        if (Input.GetMouseButton(0))
        {
            if (curDelay >= gunDelay - (0.05f * GunDelayLevel))
            {
                GunPoses.ToList().ForEach(item =>
                {
                    Projectile temp = ShootProjectile(WeaponState.MACHINEGUN, item.position, item.rotation * Quaternion.Euler(90, 0, 0) * Quaternion.Euler(Random.insideUnitSphere * (gunSpread - (0.05f * GunSpreadLevel))));
                    temp.damage = gunDamage + GunDamageLevel;
                });
                Particles[0].Play();
                Particles[1].Play();
                SoundManager.Instance.PlaySound("Bullet");
                curDelay = 0f;
            }
            GunsContainer.ToList().ForEach(item => item.transform.Rotate(Vector3.forward * Time.deltaTime * (200f * (GunDelayLevel + 1))));
            curDelay += Time.deltaTime;
        }
    }

    void ChangeState()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) curState = WeaponState.MACHINEGUN;
        if (Input.GetKeyDown(KeyCode.Alpha2)) curState = WeaponState.MISSILE;
        if (Input.GetKeyDown(KeyCode.Alpha3)) curState = WeaponState.HOMING;
    }

    void GunFollow()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 300f, ableLayer))
        {
            Guns.ToList().ForEach(item =>
            {
                item.transform.LookAt(hit.point);
            });
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isBulletProof)
        {
            if (other.CompareTag("Enemy"))
            {
                if (other.GetComponentInParent<Entity>().isMonster)
                    OnDamage(other.GetComponentInParent<Entity>().damage);
            }

            if (other.CompareTag("EnemyBullet"))
            {
                OnDamage(other.GetComponentInParent<EnemyBullet>().damage);
                Destroy(other.gameObject);
            }
        }
    }

    void OnDamage(float damage)
    {
        SoundManager.Instance.PlaySound("Hit");
        GameManager.Instance.Hp -= damage;
        StartCoroutine(CameraShake());
        StartCoroutine(BulletProofCoroutine());
    }

    IEnumerator BulletProofCoroutine()
    {
        isBulletProof = true;
        yield return new WaitForSeconds(1.5f);
        isBulletProof = false;
    }

    IEnumerator CameraShake()
    {
        float timer = 0.3f;

        while (timer > 0f)
        {
            cam.transform.localPosition = originPos + Random.insideUnitSphere / 2f;
            timer -= Time.deltaTime;
            yield return null;
        }

        cam.transform.localPosition = originPos;
    }
}
