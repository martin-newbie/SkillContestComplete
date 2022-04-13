using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class TitleManager : MonoBehaviour
{

    public TitleRanking ranking;
    public GameObject How2Play;
    public Transform introParent;
    public GameObject IntroObject;
    public List<GameObject> introObjects = new List<GameObject>();
    public GameObject TitleObj;
    public GameObject IntroObj;

    [Header("Intro UI")]
    public Text Logs;
    public Text Goal;
    public Text Ready;
    public Text Percent;
    public Text LauchReady;
    public Text LauchBegin;
    public GameObject Cam;
    bool trigger;

    void Start()
    {
        TitleObj.SetActive(true);
        IntroObj.SetActive(false);
        IntroInit();
    }

    void IntroInit()
    {
        for (int i = 0; i < 15; i++)
        {
            GameObject temp = Instantiate(IntroObject, introParent);
            temp.transform.localPosition = new Vector3(0, 0, 0.1f + 0.3f * (i + 1));
            temp.gameObject.SetActive(false);
            introObjects.Add(temp);
        }
    }

    void Update()
    {
        if (trigger)
        {
            Cam.transform.Translate(Vector3.forward * 300f * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.F1)) SceneManager.LoadScene("InGameScene");
    }

    public void GameExit()
    {
#if UNITY_EDITOR

#else
        Application.Quit();
#endif

    }

    public void GameStart()
    {
        StartCoroutine(Intro());
    }
    IEnumerator Intro()
    {
        TitleObj.SetActive(false);
        IntroObj.SetActive(true);

        List<string> messages = new List<string>();
        messages.Add(Logs.text);
        Logs.text = "";
        messages.Add(Goal.text);
        Goal.text = "";
        messages.Add(Ready.text);
        Ready.text = "";
        messages.Add(Percent.text);
        Percent.text = "";
        messages.Add(LauchReady.text);
        LauchReady.text = "";
        messages.Add(LauchBegin.text);
        LauchBegin.text = "";

        SoundManager.Instance.PlaySound("LightOn");
        yield return new WaitForSeconds(1f);

        List<string> logsTxt = messages[0].Split('\n').ToList();

        for (int i = 0; i < logsTxt.Count; i++)
        {
            Logs.text += logsTxt[i] + "\n";
            SoundManager.Instance.PlaySound("Beep");
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < messages[1].Length; i++)
        {
            Goal.text += messages[1][i];
            SoundManager.Instance.PlaySound("Beep");
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < messages[2].Length; i++)
        {
            Ready.text += messages[2][i];
            SoundManager.Instance.PlaySound("Beep");
            yield return new WaitForSeconds(0.01f);
        }

        for (int i = 0; i < 100; i++)
        {
            Percent.text = messages[3] + (i + 1).ToString() + "%";
            SoundManager.Instance.PlaySound("Beep");
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < messages[4].Length; i++)
        {
            LauchReady.text += messages[4][i];
            SoundManager.Instance.PlaySound("Beep");
            yield return new WaitForSeconds(0.01f);
        }

        for (int i = 5; i >= 0; i--)
        {
            LauchBegin.text = messages[5] + i.ToString() + "s";
            SoundManager.Instance.PlaySound("Beep");
            yield return new WaitForSeconds(1f);
        }

        StartCoroutine(IntroEffect());
    }

    IEnumerator IntroEffect()
    {
        for (int i = 0; i < introObjects.Count; i++)
        {
            introObjects[i].SetActive(true);
            SoundManager.Instance.PlaySoundSurround("LightOn", false, introObjects[i].transform.position);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.PlaySound("JetEngine");
        trigger = true;

        yield return new WaitForSeconds(2f);
        IntroObj.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        SoundManager.Instance.PlaySound("Beep");
        IntroObj.SetActive(true);

        yield return new WaitForSeconds(0.3f);
        IntroObj.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        SoundManager.Instance.PlaySound("Beep");
        IntroObj.SetActive(true);

        yield return new WaitForSeconds(1f);
        IntroObj.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene("InGameScene");
    }

    public void Ranking()
    {
        SoundManager.Instance.PlaySound("UIopen");
        ranking.gameObject.SetActive(true);
        ranking.Init();
    }

    public void HowToPlay()
    {
        SoundManager.Instance.PlaySound("UIopen");
        How2Play.SetActive(true);
    }

    public void OffH2P()
    {
        SoundManager.Instance.PlaySound("UIclose");
        How2Play.SetActive(false);
    }

}
