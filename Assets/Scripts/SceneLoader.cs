using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    //TODO: add slight delay to loading. 
    //Loading instantly interrupts audio, and looks bad. 
    //If we add a slight delay, we could add a fade out and allow sound to continue
    public static void loadOfflineScene()
    {
        Debug.Log("loading OfflineScene");
        SceneManager.LoadScene("OfflineScene");
    }

    public static void loadOnlineMultiplayer()
    {
        Debug.Log("loading OnlineMultiplayer");
        SceneManager.LoadScene("OnlineMultiplayer");
    }
}
