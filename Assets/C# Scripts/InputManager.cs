using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown fullscreenModeDropdown;
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Slider sensSlider;
    [SerializeField] private TMP_InputField sensInput;


    void Start()
    {
        InitializeRes();
        LoadOptionRes();
        SelectCurrentResolution();

        LoadQualityOption();
        SelectCurrentQualityLevel();

        LoadFullscreen();
        SetSensitivity(PlayerPrefs.GetFloat("MouseSensitivity", 1));
    }

    public void ApplyGraphicSettings()
    {
        ApplyFullscreen();
        UpdateResolution();
        ApplyQuality();

        SaveFullscreenValue();
        SaveOptionRes();
        SaveQualityLevel();
    }

    public void SetSensitivity(string sens)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", float.Parse(sens));
        sensInput.text = sens;
        sensSlider.value = float.Parse(sens);

        if(Look.Instance != null) { Look.Instance.mouseSensitivity = float.Parse(sens); }
    }

    public void SetSensitivity(float sens)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sens);
        sensInput.text = sens.ToString();
        sensSlider.value = sens;

        if(Look.Instance != null) { Look.Instance.mouseSensitivity = sens; }
    }

    public void SetVolume(float vol)
    {
        audioMixer.SetFloat("Volume", vol);
    }

    public void SelectCurrentQualityLevel()
    {
        qualityDropdown.value = QualitySettings.GetQualityLevel();
    }

    void LoadQualityOption()
    {
        qualityDropdown.value = PlayerPrefs.GetInt("QualityIndex");
        QualitySettings.SetQualityLevel(qualityDropdown.value);
    }
    
    void LoadFullscreen()
    {
        fullscreenModeDropdown.value = PlayerPrefs.GetInt("Fullscreen", 0);
        ApplyFullscreen();
    }

    void SaveQualityLevel()
    {
        PlayerPrefs.SetInt("QualityIndex", qualityDropdown.value);
    }
    
    void SaveFullscreenValue()
    {
        PlayerPrefs.SetInt("Fullscreen", fullscreenModeDropdown.value);
    }

    void ApplyFullscreen()
    {
        switch (fullscreenModeDropdown.value)
        {
            case 0: Screen.fullScreenMode = FullScreenMode.FullScreenWindow; break;
            case 1: Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; break;
            case 2: Screen.fullScreenMode = FullScreenMode.Windowed; break;
        }
    }

    public void ApplyQuality()
    {
        QualitySettings.SetQualityLevel(qualityDropdown.value);
        print(QualitySettings.GetQualityLevel());
    }

    private void LoadOptionRes()
    {
        string res = PlayerPrefs.GetString("Resolution", string.Empty);

        for (int i = 0; i < resolutionDropdown.options.Count; i++)
        {
            if (resolutionDropdown.options[i].text == res)
            {
                resolutionDropdown.value = i;
            }
        }
    }

    private void SelectCurrentResolution()
    {
        for (int i = 0; i < resolutionDropdown.options.Count; i++)
        {
            if (resolutionDropdown.options[i].text == Screen.currentResolution.ToString())
            {
                resolutionDropdown.value = i;
            }
        }
    }

    public void SaveOptionRes()
    {
        PlayerPrefs.SetString("Resolution", resolutionDropdown.options[resolutionDropdown.value].text);
    }

    void InitializeRes()
    {
        List<TMP_Dropdown.OptionData> optionData = new List<TMP_Dropdown.OptionData>();

        foreach (Resolution resolution in Screen.resolutions)
        {
            optionData.Add(new TMP_Dropdown.OptionData(resolution.ToString()));
        }

        optionData.Reverse();
        resolutionDropdown.AddOptions(optionData);
    }

    public void UpdateResolution()
    {
        int width = int.Parse(PlayerPrefs.GetString("Resolution").Split(' ')[0]);
        int height = int.Parse(PlayerPrefs.GetString("Resolution").Split(' ')[2]);
        int refreshRate = int.Parse(PlayerPrefs.GetString("Resolution").Split('@')[1].Split('H')[0]);

        Screen.SetResolution(width, height, Screen.fullScreenMode, refreshRate);
        Debug.Log(Screen.currentResolution);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenUrl(string url)
    {
        Application.OpenURL(url);
    }

    public void ChangeScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}
