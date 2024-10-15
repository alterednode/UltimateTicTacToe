using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
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
