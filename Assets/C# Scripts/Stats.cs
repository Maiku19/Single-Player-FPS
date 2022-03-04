using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public string playerName;
    public float Score { get { return GetScore(); } }

    public int kills = 0;
    public int deaths = 0;
    public int assists = 0;
    public int teamId = 0;

    private void Start()
    {
        playerName = name;
        if(CompareTag("Team1")) { teamId = 1; }
        else if(CompareTag("Team2")) { teamId = 2; }
    }

    float GetScore()
    {
        switch (GameManager.gameMode)
        {
            case GameManager.GameMode.TeamDeathMatch:
                if(deaths == 0) { return 0; }
                return kills / deaths;
        }

        return 0;
    }
}
