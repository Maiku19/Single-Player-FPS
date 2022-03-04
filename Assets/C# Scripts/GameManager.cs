using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    static GameManager _Instance;
    public static GameManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new GameManager();
                return _Instance;
            }

            return _Instance;
        }
    }

    #endregion

    public enum GameMode
    {
        TeamDeathMatch
    }

    public static GameMode gameMode = GameMode.TeamDeathMatch;

    void Awake()
    {
        _Instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.F1) && Time.timeScale == 1)
        {
            Time.timeScale = .1f;
        }
        else if(Input.GetKeyDown(KeyCode.F1))
        {
            Time.timeScale = 1f;
        }*/
    }
}
