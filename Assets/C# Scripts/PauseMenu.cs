using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    bool paused = false;
    [SerializeField] private GameObject menuHolder;
    [SerializeField] Look lookScript;

    void Update()
    {
        OnPauseStatusChanged();
    }

    private void OnPauseStatusChanged()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) { return; }

        paused = !paused;
        menuHolder.SetActive(paused);

        if (paused)
        {
            Time.timeScale = 0.0000000001f;
            Cursor.lockState = CursorLockMode.None;
            lookScript.enabled = false;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            lookScript.enabled = true;
            Cursor.visible = false;
        }
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
