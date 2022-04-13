using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{

    public Text Score;
    public Button nextBtn;
    public float score;

    void Start()
    {
    }

    void Update()
    {

    }

    public void Init(float score)
    {
        SoundManager.Instance.PlaySound("Fail");
        this.score = score;
        StartCoroutine(GameOverPrint());
    }

    public void End()
    {
        GameManager.Instance.GameOverStage();
    }

    IEnumerator GameOverPrint()
    {
        yield return StartCoroutine(TextCounting(Score, 0, score));
        yield return new WaitForSeconds(1f);

        nextBtn.gameObject.SetActive(true);
    }

    IEnumerator TextCounting(Text text, float cur, float max)
    {
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
