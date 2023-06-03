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
        // BRUH! looking at my old code makes my want to kms

        if (!matchInProgress) { return; }

        if(id == 1)
        {
            Score1 += score;
            GameObject scoreCounter = GameObject.FindGameObjectWithTag("Score1");
            scoreCounter.GetComponent<TextMeshProUGUI>().text = Score1.ToString();
            scoreCounter.transform.parent.GetComponent<UnityEngine.UI.Slider>().value = Score1;
            scoreCounter.transform.parent.GetComponent<UnityEngine.UI.Slider>().maxValue = victoryConditionTeam1;
        }
        else
        {
            Score2 += score; 
            GameObject scoreCounter = GameObject.FindGameObjectWithTag("Score2");
            scoreCounter.GetComponent<TextMeshProUGUI>().text = Score2.ToString();
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
        else if(Score2 >= victoryConditionTeam2) { GameObject.FindGameObjectWithTag("EndGameScreen").GetComponent<EndGameScreen>().OnTeam2Victory(); }
    }

    private static void Initialize()
    {
        // Reset variables on scene reload (because they are static variables)
        matchInProgress = true;
        Score1 = 0;
        Score2 = 0;

        AddPlayers(ref team1Players, "Team1");
        AddPlayers(ref team2Players, "Team2");

        static void AddPlayers(ref Stats[] teamStats, string teamTag)
        {
            GameObject[] team = GameObject.FindGameObjectsWithTag(teamTag);
            teamStats = new Stats[team.Length];

            for (int i = 0; i < team.Length; i++)
            {
                teamStats[i] = team[i].GetComponent<Stats>();
            }
        }
    }

    public static Stats[] GetSortedTeam(int id)
    {
        SortArray(id);
        return id == 1 ? team1Players : team2Players;
    }

    static void SortArray(int id)
    {
        List<Stats> array = new(id == 1 ? team1Players : team2Players);

        array.Sort((a, b) =>
        { 
            if(a.Score > b.Score) { return -1; } 
            else if(a.Score == b.Score) { return 0;}
            else { return 1;}
        });

        if (id == 1) { SetArray(ref team1Players); } else { SetArray(ref team2Players); }

        void SetArray(ref Stats[] stats) => stats = array.ToArray();
    }
}
