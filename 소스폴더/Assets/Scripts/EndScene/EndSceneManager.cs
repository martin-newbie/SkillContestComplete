using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndSceneManager : MonoBehaviour
{
    public float Score;
    public InputField input;
    public Text scoreTxt;
    public Text previously;

    private void Start()
    {
        Score = StatusManager.Instance.stage1Score + StatusManager.Instance.stage2Score;
        InitPreviously();
        StartCoroutine(TextCounting(scoreTxt, 0, Score));
    }

    void InitPreviously()
    {
        int idx = 1;
        foreach (var item in StatusManager.Instance.Users)
        {
            previously.text += idx.ToString() + "). " + item.Name + ": " + item.Score + "\n";
            idx++;
        }
    }

    IEnumerator TextCounting(Text text, float cur, float max)
    {
        float duration = 0.5f;
        float offset = (max - cur) / duration;

        while (cur < max)
        {
            cur += offset * Time.deltaTime;
            text.text = string.Format("{0:0}", cur);
            yield return null;
        }

        cur = max;
        text.text = string.Format("{0:0}", cur);
    }

    public void Submit()
    {
        if(input.text.Length > 3)
        {
            StatusManager.Instance.AddNewUser(input.text, Score);
            StatusManager.Instance.curStage = 0;
            UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
        }
        else
        {
            PrintLog("pleas enter the text less than 3 characters");
        }
    }

    public void PrintLog(string log)
    {
        input.placeholder.GetComponent<Text>().text = log;
    }
}
