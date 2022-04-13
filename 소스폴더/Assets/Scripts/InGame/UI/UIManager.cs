using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{

    [Header("WeaponStats")]
    public Image[] Guns = new Image[3];
    public Text Cur;
    public Text Max;
    public Text Inventory;

    [Header("Objects")]
    public GameObject Texts;
    public GameObject Infinity;
    public Image PainImg;
    public Image HpImg;

    [Header("Loading")]
    public Image M_Loading;
    public Image H_Loading;

    [Header("Levels")]
    public Text[] Levels = new Text[3];

    [Header("Player Status")]
    public Image PlayerHpImg;
    public Text PlayerHpTxt;
    public Image PlayerPpImg;
    public Text PlayerPpTxt;
    public Text ScoreText;

    [Header("Monster Hp")]
    public GameObject MonsterHpContainer;
    public Image MonsterProfile;
    public Image MonsterHpFade;
    public Image MonsterHp;
    public Text maxHp;
    public Text curHp;
    public bool IsMonster;
    float hp;
    float HpFill;

    [Header("Game Result")]
    public GameOver gameover;
    public GameClear gameClear;

    void Start()
    {
        MonsterHpContainer.SetActive(false);
    }

    void Update()
    {
    }

    public void GameOver(float score)
    {
        gameover.gameObject.SetActive(true);
        gameover.Init(score);
    }

    public void GameClear(float score, float hp, float pp)
    {
        gameClear.gameObject.SetActive(true);
        gameClear.Init(score, hp, pp);
    }

    public void SetBossMonsterHp(float _hp, float _max)
    {
        IsMonster = true;
        MonsterHpContainer.SetActive(true);
        MonsterHp.fillAmount = _hp;
        hp = _hp;

        curHp.text = ((int)(_hp * _max)).ToString();
        maxHp.text = _max.ToString();

        HpFill = Mathf.Lerp(HpFill, hp, Time.deltaTime * 10f);
        MonsterHpFade.fillAmount = HpFill;
    }

    public void SetScore(float score)
    {
        ScoreText.text = string.Format("{0:#00}", score);
    }

    public void SetCurWeapon(int idx)
    {
        Guns.ToList().ForEach(item =>
        {
            Color temp = item.color;
            temp.a = 0.2f;
            item.color = temp;
        });

        Color temp = Guns[idx].color;
        temp.a = 0.6f;
        Guns[idx].color = temp;
    }

    public void SetWeaponStatus(bool infinity, int cur = 0, int max = 0, int inventory = 0)
    {
        Texts.SetActive(!infinity);
        Infinity.SetActive(infinity);

        if (!infinity)
        {
            Cur.text = cur.ToString();
            Max.text = max.ToString();
            Inventory.text = inventory.ToString();
        }
    }

    public void SetMLoading(float fill)
    {
        M_Loading.fillAmount = fill;
    }

    public void SetHLoading(float fill)
    {
        H_Loading.fillAmount = fill;
    }

    public void SetWeaponLevel(int damage, int delay, int spread)
    {
        Levels[0].text = damage.ToString();
        Levels[1].text = delay.ToString();
        Levels[2].text = spread.ToString();
    }

    public void SetPlayerStatus(float hp, float pp)
    {
        PlayerHpImg.fillAmount = hp / GameManager.Instance.MaxHp;
        HpImg.color = new Color(1, 1, 1, 1 - (hp / GameManager.Instance.MaxHp));
        PlayerHpTxt.text = (int)hp + "%";

        PlayerPpImg.fillAmount = pp / 100f;
        PainImg.color = new Color(1, 1, 1, pp / 100f);
        PlayerPpTxt.text = (int)pp + "%";
    }
}
