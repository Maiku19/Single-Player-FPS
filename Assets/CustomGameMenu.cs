using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomGameMenu : MonoBehaviour
{
    [SerializeField] private MapSelector mapSelectionMenu;

    private int playersOnTeam1;
    private int playersOnTeam2;

    private bool useDynamicDifficultyTeam1 = true;
    private bool useDynamicDifficultyTeam2 = true;
    private float difficultyTeam1 = 10;
    private float difficultyTeam2 = 10;

    public void StartGame()
    {
        PlayerPrefs.SetInt("Players Team1", playersOnTeam1);
        PlayerPrefs.SetInt("Players Team2", playersOnTeam2);

        PlayerPrefs.SetInt("Use Dynamic Difficulty Team1", useDynamicDifficultyTeam1 ? 1 : 0);
        PlayerPrefs.SetInt("Use Dynamic Difficulty Team2", useDynamicDifficultyTeam2 ? 1 : 0);

        PlayerPrefs.SetFloat("Difficulty Team1", difficultyTeam1);
        PlayerPrefs.SetFloat("Difficulty Team2", difficultyTeam2);

        SceneManager.LoadScene(mapSelectionMenu.selectedMap);
    }
}
