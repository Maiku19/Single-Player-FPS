using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionDropdown : MonoBehaviour
{
    private TMP_Dropdown dropdown;

    void Start()
    {
        Initialize();
        LoadOption();
        SelectCurrentResolution();
        SaveOption();
    }

    private void LoadOption()
    {
        string res = PlayerPrefs.GetString("Resolution", string.Empty);

        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == res)
            {
                dropdown.value = i;
            }
        }
    }

    private void SelectCurrentResolution()
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == Screen.currentResolution.ToString())
            {
                dropdown.value = i;
            }
        }
    }

    public void SaveOption()
    {
        PlayerPrefs.SetString("Resolution", dropdown.options[dropdown.value].text);
    }

    void Initialize()
    {
        if (GetComponent<TMP_Dropdown>()) { dropdown = GetComponent<TMP_Dropdown>(); }
        List<TMP_Dropdown.OptionData> optionData = new List<TMP_Dropdown.OptionData>();

        foreach (Resolution resolution in Screen.resolutions)
        {
            optionData.Add(new TMP_Dropdown.OptionData(resolution.ToString()));
        }

        optionData.Reverse();
        dropdown.AddOptions(optionData);
    }

    public void UpdateResolution()
    {
        int width = int.Parse(PlayerPrefs.GetString("Resolution").Split(' ')[0]);
        int height = int.Parse(PlayerPrefs.GetString("Resolution").Split(' ')[2]);

        Screen.SetResolution(width, height, Screen.fullScreenMode);
    }
}
