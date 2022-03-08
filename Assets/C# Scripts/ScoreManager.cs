using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static Stats[] SortedTeam1 { get { return GetSortedTeam(1); } }
    public static Stats[] SortedTeam2 { get { return GetSortedTeam(2); } }

    public static float Score1 { get; set; }
    public static float Score2 { get; set; }

    public float victoryConditionTeam1;
    public float victoryConditionTeam2;

    static Stats[] team1Players;
    static Stats[] team2Players;

    static ScoreManager _Instance;
    public static ScoreManager Instance
    {
        get { return _Instance; }
    }

    public float timeLeft = 0;
    public float matchTime = 600; // 10min

    static bool matchInProgress = true;

    private void Awake()
    {
        _Instance = this;
    }

    private void Start()
    {
        victoryConditionTeam1 = PlayerPrefs.GetFloat("Score Limit Team1", victoryConditionTeam1);
        victoryConditionTeam2 = PlayerPrefs.GetFloat("Score Limit Team2", victoryConditionTeam2);

        matchTime = PlayerPrefs.GetFloat("Time Limit", matchTime);

        Initialize();
        StartCoroutine(IncrementTimer());
    }


    // ----------

    public void OnPlayerDied(GameObject player)
    {
        switch (GameManager.gameMode)
        {
            case GameManager.GameMode.TeamDeathMatch:
                if (player.CompareTag("Team1")) { IncrementScore(2, 1); }
                else if (player.CompareTag("Team2")) { IncrementScore(1, 1); }
                break;
            default:
                break;
        }
    }

    public void IncrementScore(int id, float score)
    {
        if (!matchInProgress) { return; }

        if(id == 1)
        {
            Score1 += score;
            GameObject scoreCounter = GameObject.FindGameObjectWithTag("Score1");
            scoreCounter.GetComponent<TMPro.TextMeshProUGUI>().text = Score1.ToString();
            scoreCounter.transform.parent.GetComponent<UnityEngine.UI.Slider>().value = Score1;
            scoreCounter.transform.parent.GetComponent<UnityEngine.UI.Slider>().maxValue = victoryConditionTeam1;
        }
        else
        {
            Score2 += score; 
            GameObject scoreCounter = GameObject.FindGameObjectWithTag("Score2");
            scoreCounter.GetComponent<TMPro.TextMeshProUGUI>().text = Score2.ToString();
            scoreCounter.transform.parent.GetComponent<UnityEngine.UI.Slider>().value = Score2;
            scoreCounter.transform.parent.GetComponent<UnityEngine.UI.Slider>().maxValue = victoryConditionTeam2;
        }

        

        EndGameIfAble();
    }

    void EndGameIfAble()
    {
        if (!matchInProgress) { return; }

        if(Score1 >= victoryConditionTeam1 || Score2 >= victoryConditionTeam2)
        {
            StartCoroutine(EndMatch());
        }
    }

    IEnumerator IncrementTimer()
    {
        TextMeshProUGUI matchTimer = null;
        timeLeft = matchTime;

        while (timeLeft > 0) 
        {
            yield return null;
            timeLeft -= Time.deltaTime;

            if(matchTimer == null) matchTimer = GameObject.FindGameObjectWithTag("MatchTimer").GetComponent<TextMeshProUGUI>();
            matchTimer.text = System.TimeSpan.FromSeconds(timeLeft).ToString(@"mm\:ss");
        }

        print("Match Over!");
        StartCoroutine(EndMatch());
    }

    IEnumerator EndMatch()
    {
        if (!matchInProgress) { yield break; }
        matchInProgress = false;

        DeclareWiner();

        while (Time.timeScale > 0.05f)
        {
            yield return null;
            Time.timeScale -= .2f * Time.unscaledDeltaTime;
        }

        Time.timeScale = 0.05f;
    }

    void DeclareWiner()
    {
        if(Score1 >= victoryConditionTeam1) { GameObject.FindGameObjectWithTag("EndGameScreen").GetComponent<EndGameScreen>().OnTeam1Victory(); }
        if(Score2 >= victoryConditionTeam2) { GameObject.FindGameObjectWithTag("EndGameScreen").GetComponent<EndGameScreen>().OnTeam2Victory(); }
    }

    private static void Initialize()
    {
        // IDK y but variables don't reset after scene change
        matchInProgress = true;
        Score1 = 0;
        Score2 = 0;
        team1Players = new Stats[0];
        team2Players = new Stats[0];

        GameObject[] t1 = GameObject.FindGameObjectsWithTag("Team1");
        team1Players = new Stats[t1.Length];

        for (int i = 0; i < t1.Length; i++)
        {
            team1Players[i] = t1[i].GetComponent<Stats>();
        }

        GameObject[] t2 = GameObject.FindGameObjectsWithTag("Team2");
        team2Players = new Stats[t2.Length];

        for (int i = 0; i < t2.Length; i++)
        {
            team2Players[i] = t1[i].GetComponent<Stats>();
        }
    }

    public static Stats[] GetSortedTeam(int id)
    {
        SortArray(id);
        return id == 1 ? team1Players : team2Players;
    }

    static void SortArray(int id)
    {
        Stats[] array = id == 1 ? team1Players : team2Players;
        bool sorted;
        
        do
        {
            sorted = true;

            for (int i = 0; i < array.Length - 1; i++)
            {
                Stats current = array[i];
                Stats next = array[i + 1];

                if(current.Score < next.Score)
                {
                    sorted = false;
                    array = Mike.MikeArray.Swap(array, i, i + 1);
                }
            }
        }
        while (!sorted);
    }
}
