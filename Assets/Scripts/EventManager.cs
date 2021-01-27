using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public int button = 0;

    public static bool isClick = false;

    PlayerManager playerManager;

    void Start()
    {
        //playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        //Debug.Log("playerManager:" + playerManager);
    }

    public void OnNegoButton()
    {
        isClick = true;
        button = 1;
        //playerManager.karmaPoint += 1;
        Debug.Log(button);
    }
    public void OnBattleButton()
    {
        isClick = true;
        button = 2;
        //playerManager.karmaPoint -= 2;
        Debug.Log(button);
    }
    public void OnEscButton()
    {
        isClick = true;
        button = 3;
        //playerManager.karmaPoint += 0;
        Debug.Log(button);
    }
}
