using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatusManager : Singleton<StatusManager>
{

    public List<User> Users = new List<User>();

    public float stage1Score;
    public float stage2Score;
    public int curStage;

    public GameData data;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        AddNewUser("im the best", 50000);
        AddNewUser("kong", 22222);
        AddNewUser("yield", 3000);
        AddNewUser("martin", 500);
        AddNewUser("newbie", 100);

        LoadGameData();
    }

    void Start()
    {
        SceneManager.LoadScene("TitleScene");
    }

    void LoadGameData()
    {
        if(File.Exists(Application.dataPath + "Data.txt"))
        {
            string data = File.ReadAllText(Application.dataPath + "Data.txt");
            this.data = JsonUtility.FromJson<GameData>(data);
        }
        else
        {
            data = new GameData();
            string save = JsonUtility.ToJson(data, true);
            File.WriteAllText(Application.dataPath + "Data.txt", save);
        }
    }

    void Update()
    {
        
    }

    public void Init()
    {
        stage1Score = 0f;
        stage2Score = 0f;
    }

    public void GameEndScene()
    {
        SceneManager.LoadScene("EndScene");
    }

    public void AddNewUser(string _name, float _score)
    {
        User newUser = new User(_name, _score);
        Users.Add(newUser);
    }
}

[System.Serializable]
public class User
{
    public string Name;
    public float Score;

    public User(string name, float score)
    {
        Name = name;
        Score = score;
    }
}

[System.Serializable]
public class GameData
{
    public float Hp = 100f;
    public float Dmg = 5f;
    public float MS = 50f; //move speed

    public float B_Hp = 15f; //bacteria
    public float V_Hp = 55f; //Virus
    public float G_Hp = 45f; //Germ
    public float C_Hp = 140f; //Cancer;

    public float Boss_1_Hp = 1500f;
    public float Boss_2_Hp = 5000f;
    public float Boss_Dmg = 15f;

    public float E_Dmg_1 = 3f; //enemy damage 1
    public float E_Dmg_2 = 10f; //enemy damage 2

    public float W_Chance = 30f; //white cell spawn chance
    public float R_Chance = 60f; //red cell spawn chance
}