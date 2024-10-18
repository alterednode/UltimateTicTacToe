using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSettings : MonoBehaviour
{
    public GameObject[] settingsItems;
    private int[] themeColors = { 0xFFFFFF, 0x444444 };

    public Camera sceneCamera;

    public Sprite[] themedSprites;

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
        //retrieve the theme from the playerprefs
        int userThemeNum = PlayerPrefs.GetInt("Theme");

        //get the invert of the theme for ui elements
        int userThemeInvertNum = (userThemeNum + 1) % themeColors.Length;

        byte themeColor = (byte)themeColors[userThemeNum];
        byte themeInvertColor = (byte)themeColors[userThemeInvertNum];

        //apply theme to camera background
        sceneCamera.backgroundColor = new Color32(themeColor, themeColor, themeColor, 255);

        // Find all GameObjects with SpriteRenderer in the scene
        SpriteRenderer[] allSpriteRenderers = FindObjectsOfType<SpriteRenderer>();

        // Loop through all SpriteRenderers and check their sprite
        foreach (SpriteRenderer spriteRenderer in allSpriteRenderers)
        {
            foreach (Sprite themedSprite in themedSprites)
            {
                // If the sprite matches any of the sprites we've chosen in themedSprites, change its color

                if (spriteRenderer.sprite == themedSprite)
                {
                    spriteRenderer.color = new Color32(themeInvertColor, themeInvertColor, themeInvertColor, 255);
                }
            }
        }


    }
}
