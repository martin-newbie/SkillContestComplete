using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClear : MonoBehaviour
{

    public Text ScoreTxt;
    public Text HpTxt;
    public Text PpTxt;
    public Text TotalTxt;
    public Button NextButton;

    float score, hp, pp;

    void Start()
    {
    }

    void Update()
    {

    }

    public void Init(float _score, float _hp, float _pp)
    {
        SoundManager.Instance.PlaySound("Victory");
        score = _score;
        hp = _hp;
        pp = _pp;
        StartCoroutine(GameClearCoroutine());
    }

    IEnumerator GameClearCoroutine()
    {
        yield return CountAnimation(0, score, ScoreTxt);
        yield return new WaitForSeconds(1f);

        yield return CountAnimation(0, hp, HpTxt);
        yield return new WaitForSeconds(1f);

        yield return CountAnimation(0, pp, PpTxt);
        yield return new WaitForSeconds(1f);

        yield return CountAnimation(0, score + hp + pp, TotalTxt);
        yield return new WaitForSeconds(1f);

        NextButton.gameObject.SetActive(true);
    }

    public void Next()
    {
        GameManager.Instance.NextStage();
    }

    IEnumerator CountAnimation(float cur, float max, Text text)
    {
        text.gameObject.SetActive(true);
        float duration = 0.5f;
        float offset = (max - cur) / duration;

        while (cur < max)
        {
            SoundManager.Instance.PlaySound("count");
            cur += offset * Time.deltaTime;
            text.text = string.Format("{0:0}", cur);
            yield return null;
        }

        cur = max;
        text.text = string.Format("{0:0}", cur);
    }
}
