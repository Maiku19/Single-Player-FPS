using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public string PlayerName;
    public float Score { get { return GetScore(); } }

    public int Kills = 0;
    public int Deaths = 0;
    public int Assists = 0;
    public int TeamId = 0;

    private void Start()
    {
        PlayerName = name;
        if(CompareTag("Team1")) { TeamId = 1; }
        else if(CompareTag("Team2")) { TeamId = 2; }
    }

    float GetScore()
    {
        switch (GameManager.gameMode)
        {
            case GameManager.GameMode.TeamDeathMatch:
                if(Deaths == 0) { return Kills * 100; }
                return (Kills * 100) / Deaths;
        }

        return 0;
    }
}
