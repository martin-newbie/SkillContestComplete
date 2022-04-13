using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{

    [Header("UI")]
    public UnityEngine.UI.Image blackImg;

    [Header("Game Status")]
    public bool isBoss;
    public bool gameOver;

    [Header("Prefabs")]
    public TextMesh ScoreObject;

    [Header("Player Status")]
    public Player player;
    public float MaxHp;
    public float Hp;
    public float Pp; //pain point
    public float Score;

    public int Stage = 0; //stage 1: 0

    [Header("Monsters")]
    public Entity Bacteria;
    public Entity Virus;
    public Entity Germ;
    public Entity Cancer;

    [Header("Boss")]
    public Stage1Boss Boss_1;
    public GameObject Boss_2;

    [Header("Friendly")]
    public Entity WhiteCell;
    public Entity RedCell;//정녕 아군이 맞는건가? sombody help me!

    [Header("Stage 1 Map")]
    public GameObject Stage1;
    public float moveSpeed;
    public MeshRenderer ring;
    float offset;

    [Header("Stage 2 Map")]
    public GameObject Stage2;
    public MeshRenderer plane;

    [Header("Camera Settings")]
    public CinemachineVirtualCamera mainCam;
    public CinemachineVirtualCamera bossCam;

    bool gameTrigger;

    void Start()
    {

        MaxHp = StatusManager.Instance.data.Hp;
        Hp = MaxHp;

        Stage = StatusManager.Instance.curStage;

        Pp = (Stage * 2 + 1) * 10f;

        switch (Stage)
        {
            case 0:
                Stage1.gameObject.SetActive(true);
                break;
            case 1:
                Stage2.gameObject.SetActive(true);
                break;
        }
        StartCoroutine(GameStart());
    }


    IEnumerator GameStart()
    {
        yield return StartCoroutine(FadeOut());

        Coroutine temp = Stage == 0 ? StartCoroutine(Stage1Pattern()) : StartCoroutine(Stage2Pattern());
        StartCoroutine(FriendlySpawn());
        gameTrigger = true;
    }

    IEnumerator FadeOut()
    {
        Color color = new Color(0, 0, 0, 1);
        blackImg.gameObject.SetActive(true);
        float offset = 1 / 0.5f;
        while (color.a > 0)
        {
            color.a -= offset * Time.deltaTime;
            blackImg.color = color;
            yield return null;
        }
        color.a = 0f;
        blackImg.color = color;
        blackImg.gameObject.SetActive(false);
    }

    IEnumerator FadeIn()
    {
        Color color = new Color(0, 0, 0, 0);
        blackImg.gameObject.SetActive(true);
        float offset = 1 / 0.5f;
        while (color.a < 1)
        {
            color.a += offset * Time.deltaTime;
            blackImg.color = color;
            yield return null;
        }
        color.a = 1f;
        blackImg.color = color;
        blackImg.gameObject.SetActive(false);
    }

    void Update()
    {
        UIManager.Instance.SetPlayerStatus(Hp, Pp);
        UIManager.Instance.SetScore(Score);

        switch (Stage)
        {
            case 0:
                Stage1Logic();
                break;
            case 1:
                Stage2Logic();
                break;
        }

        Clamp();

        if (gameTrigger)
            CheckGameover();
    }

    public void CinematicStart()
    {
        mainCam.gameObject.SetActive(false);
        bossCam.gameObject.SetActive(true);
    }

    public void CinematicEnd()
    {
        mainCam.gameObject.SetActive(true);
        bossCam.gameObject.SetActive(false);
    }

    void Stage2Logic()
    {
        offset += Time.deltaTime * moveSpeed;
        plane.material.SetTextureOffset("_BumpMap", new Vector2(0, -offset));

        if (player.GunDamageLevel + player.GunDelayLevel + player.GunSpreadLevel >= 9 && !isBoss)
        {
            SoundManager.Instance.PlaySound("Warning");
            isBoss = true;
            RemoveAllMonsters();
            Boss_2.gameObject.SetActive(true);
        }
    }

    void Stage1Logic()
    {
        offset += Time.deltaTime * moveSpeed;
        ring.material.SetTextureOffset("_BumpMap", new Vector2(offset, offset));

        if (player.GunDamageLevel + player.GunDelayLevel + player.GunSpreadLevel >= 6 && !isBoss)
        {
            SoundManager.Instance.PlaySound("Warning");
            isBoss = true;
            RemoveAllMonsters();
            Boss_1.gameObject.SetActive(true);
        }
    }

    void Clamp()
    {
        Hp = Mathf.Clamp(Hp, 0, MaxHp);
        Pp = Mathf.Clamp(Pp, 0, 100f);
    }

    void CheckGameover()
    {
        if ((Hp <= 0 || Pp >= 100) && !gameOver)
        {
            gameOver = true;
            player.GameOver();
            PrintScore();
        }
    }


    public void PrintScore()
    {
        StageEnd();
    }

    public void StageEnd()
    {
        if (gameOver) UIManager.Instance.GameOver(Score);
        else UIManager.Instance.GameClear(Score, Hp * 1000, (100 - Pp) * 1000);
    }

    public void GameOverStage()
    {

        if (Stage == 0) StatusManager.Instance.stage1Score = Score;
        else StatusManager.Instance.stage2Score = Score;

        StatusManager.Instance.curStage = 0;
        StartCoroutine(GameOverCoroutine());
    }

    IEnumerator GameOverCoroutine()
    {
        yield return StartCoroutine(FadeIn());
        StatusManager.Instance.GameEndScene();
    }

    public void NextStage()
    {
        StartCoroutine(NextStageCoroutine());
    }

    IEnumerator NextStageCoroutine()
    {
        yield return StartCoroutine(FadeIn());

        if (Stage == 0)
        {
            StatusManager.Instance.stage1Score = Score + Hp * 1000 + (100 - Pp) * 1000;
            StatusManager.Instance.curStage = 1;
            UnityEngine.SceneManagement.SceneManager.LoadScene("InGameScene");
        }
        else
        {
            StatusManager.Instance.stage2Score = Score + Hp * 1000 + (100 - Pp) * 1000;
            StatusManager.Instance.curStage = 0;
            StatusManager.Instance.GameEndScene();
        }
    }

    public void InstantiateWhiteCell()
    {
        Vector3 randPos = new Vector3(Random.Range(-40f, 40f), Random.Range(-15f, 35f), 100f);
        Instantiate(WhiteCell, randPos, Quaternion.identity);

    }

    public void InstantiateRedCell()
    {
        Vector3 randPos = new Vector3(Random.Range(-40f, 40f), Random.Range(-15f, 35f), 100f);
        Instantiate(RedCell, randPos, Quaternion.identity);
    }

    public void RemoveAllMonsters()
    {
        var monsters = FindObjectsOfType<Entity>();
        foreach (var item in monsters)
        {
            if (item.GetComponent<RedCell>() == null && item.GetComponent<WhiteCell>() == null)
                item.OnDamage(item.maxHp);
        }
    }

    IEnumerator FriendlySpawn()
    {

        while (true)
        {

            int chance = Random.Range(0, 100);
            if (chance <= StatusManager.Instance.data.R_Chance)
            {
                Vector3 randPos = new Vector3(Random.Range(-40f, 40f), Random.Range(-15f, 35f), 100f);
                Instantiate(RedCell, randPos, Quaternion.identity);
            }

            int chance2 = Random.Range(0, 100);
            if (chance <= StatusManager.Instance.data.W_Chance)
            {
                Vector3 randPos = new Vector3(Random.Range(-40f, 40f), Random.Range(-15f, 35f), 100f);
                Instantiate(WhiteCell, randPos, Quaternion.identity);
            }

            yield return new WaitForSeconds(3.5f);
        }
    }

    IEnumerator Stage1Pattern()
    {
        // -80~80, 0~50
        while (!isBoss)
        {
            Vector3 randPos = new Vector3(Random.Range(-40f, 40f), Random.Range(-15f, 35f), 100f);
            Entity temp = Random.Range(0, 3) == 0 ? Virus : Bacteria;
            Instantiate(temp, randPos, Quaternion.identity);
            yield return new WaitForSeconds(2f);

        }

    }

    IEnumerator Stage2Pattern()
    {
        while (!isBoss)
        {
            Vector3 randPos = new Vector3(Random.Range(-40f, 40f), Random.Range(-15f, 35f), 100f);
            Entity temp = Random.Range(0, 5) == 0 ? Cancer : Germ;
            Instantiate(temp, randPos, Quaternion.identity);
            yield return new WaitForSeconds(2f);

        }
    }
}
