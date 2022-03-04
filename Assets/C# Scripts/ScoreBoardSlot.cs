using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBoardSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI killsText;
    [SerializeField] TextMeshProUGUI deathsText;
    [SerializeField] TextMeshProUGUI assistsText;

    public string Name { get => _name; set { SetStats(value, score, kills, deaths, assists); } }
    public float Score { get => score; set { SetStats(_name, value, kills, deaths, assists); } }
    public int Kills { get => kills; set { SetStats(_name, score, value, deaths, assists); ; } }
    public int Deaths { get => deaths; set { SetStats(_name, score, kills, value, assists); } }
    public int Assists { get => assists; set { SetStats(_name, score, kills, deaths, value); } }


    private string _name;
    private float score;
    private int kills;
    private int deaths;
    private int assists;

    void SetStats(string name, float score, int kills, int deaths, int assists)
    {
        this._name = name;
        this.score = score;
        this.kills = kills;
        this.deaths = deaths;
        this.assists = assists;

        UpdateText();
    }

    void UpdateText()
    {
        nameText.text = _name;
        scoreText.text = score.ToString();
        killsText.text = kills.ToString();
        deathsText.text = deaths.ToString();
        assistsText.text = assists.ToString();
    }

    void Start()
    {
        UpdateText();
    }
}
