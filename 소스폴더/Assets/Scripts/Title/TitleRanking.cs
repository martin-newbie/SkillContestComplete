using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TitleRanking : MonoBehaviour
{
    public Text[] Ranking = new Text[5];
    public List<User> Users = new List<User>();

    void Start()
    {

    }

    public void Init()
    {
        Users = StatusManager.Instance.Users;

        var usersOrder = from user in Users
                         orderby user.Score descending
                         select user;

        List<User> topUser = usersOrder.ToList();

        for (int i = 0; i < Ranking.Length; i++)
        {
            Ranking[i].text = (i + 1).ToString() + ") " + topUser[i].Name + " : " + topUser[i].Score;
        }

    }

    public void Exit()
    {
        SoundManager.Instance.PlaySound("UIclose");
        gameObject.SetActive(false);
    }

}
