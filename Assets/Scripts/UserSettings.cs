using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSettings : MonoBehaviour
{
    public GameObject[] settingsItems;
    private int[] themeColors = { 0xFFFFFF, 0x444444 };

    public Camera sceneCamera;

    public void Start()
    {
        //set the theme to the default value if it hasn't been set yet
        if (!PlayerPrefs.HasKey("Theme"))
        {
            PlayerPrefs.SetInt("Theme", 0);
        }

        applyTheme();
    }

    public void OpenSettings()
    {
        //open the settings menu
        foreach (GameObject item in settingsItems)
        {
            item.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        //close the settings menu
        foreach (GameObject item in settingsItems)
        {
            item.SetActive(false);
        }
    }

    public void changeTheme()
    {
        int userThemeNum = PlayerPrefs.GetInt("Theme");
        userThemeNum = (userThemeNum + 1) % themeColors.Length;
        PlayerPrefs.SetInt("Theme", userThemeNum);
        applyTheme();
    }

    public void applyTheme()
    {
        int userThemeNum = PlayerPrefs.GetInt("Theme");
        byte themeColor = (byte)themeColors[userThemeNum];
        sceneCamera.backgroundColor = new Color32(themeColor, themeColor, themeColor, 255);
    }
}
