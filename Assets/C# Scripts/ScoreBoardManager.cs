using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoardManager : MonoBehaviour
{
    [SerializeField] private GameObject holder;

    [SerializeField] private RectTransform contentTeam1;
    [SerializeField] private RectTransform contentTeam2;

    [SerializeField] private GameObject template;

    ScoreBoardSlot[] slotsTeam1 = new ScoreBoardSlot[0];
    ScoreBoardSlot[] slotsTeam2 = new ScoreBoardSlot[0];

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            ShowScoreBoard();
        }
        else if(holder.activeSelf)
        {
            holder.SetActive(false);
        }
    }

    void Initialize()
    {
        template.SetActive(false);

        slotsTeam1 = new ScoreBoardSlot[ScoreManager.SortedTeam1.Length];
        slotsTeam2 = new ScoreBoardSlot[ScoreManager.SortedTeam2.Length];

        for (int i = 0; i < slotsTeam1.Length; i++)
        {
            slotsTeam1[i] = Instantiate(template, contentTeam1).GetComponent<ScoreBoardSlot>();
            slotsTeam1[i].gameObject.SetActive(true);
        }
        
        for (int i = 0; i < slotsTeam2.Length; i++)
        {
            slotsTeam2[i] = Instantiate(template, contentTeam2).GetComponent<ScoreBoardSlot>();
            slotsTeam2[i].gameObject.SetActive(true);

        }
    }

    public void ShowScoreBoard()
    {
        Stats[] team1 = ScoreManager.SortedTeam1;

        for (int i = 0; i < team1.Length; i++)
        {
            slotsTeam1[i].Name = team1[i].PlayerName;
            slotsTeam1[i].Score = team1[i].Score;
            slotsTeam1[i].Kills = team1[i].Kills;
            slotsTeam1[i].Deaths = team1[i].Deaths;
            slotsTeam1[i].Assists = team1[i].Assists;
        }
        
        Stats[] team2 = ScoreManager.SortedTeam2;

        for (int i = 0; i < team2.Length; i++)
        {
            slotsTeam2[i].Name = team2[i].PlayerName;
            slotsTeam2[i].Score = team2[i].Score;
            slotsTeam2[i].Kills = team2[i].Kills;
            slotsTeam2[i].Deaths = team2[i].Deaths;
            slotsTeam2[i].Assists = team2[i].Assists;
        }

        holder.SetActive(true);
    }
}
