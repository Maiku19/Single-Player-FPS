using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CustomGameMenu : MonoBehaviour
{
    [SerializeField] private MapSelector mapSelectionMenu;
    [Space(10)]

    [SerializeField] private TextMeshProUGUI playersOnTeam1;
    [SerializeField] private TextMeshProUGUI playersOnTeam2;
    [Space(10)]

    [SerializeField] private Toggle useDynamicDifficultyTeam1;
    [SerializeField] private Toggle useDynamicDifficultyTeam2;
    [Space(10)]

    [SerializeField] private TextMeshProUGUI difficultyTeam1;
    [SerializeField] private TextMeshProUGUI difficultyTeam2;

    public void StartGame()
    {
        PlayerPrefs.SetInt("Players Team1", int.Parse(playersOnTeam1.text));
        PlayerPrefs.SetInt("Players Team2", int.Parse(playersOnTeam2.text));

        PlayerPrefs.SetInt("Use Dynamic Difficulty Team1", useDynamicDifficultyTeam1.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Use Dynamic Difficulty Team2", useDynamicDifficultyTeam2.isOn ? 1 : 0);

        PlayerPrefs.SetFloat("Difficulty Team1", float.Parse(difficultyTeam1.text));
        PlayerPrefs.SetFloat("Difficulty Team2", float.Parse(difficultyTeam2.text));

        SceneManager.LoadScene(mapSelectionMenu.selectedMap);
    }
}
