using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OnlineManager : MonoBehaviour
{

    string serverURL = "150.230.36.239:8080";
    
    public Boolean canReachServer = true;
    private void Awake()
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.multiplayerEnabled = true;
        gameManager.canHumanPlayerPlay = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("checking server connection");
        checkServerConnection();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool checkInternetConnection ()
    {
        //TODO: ping google or something
        return false;
    }
    bool checkServerConnection()
    {
        //TODO: ping / check server status.

        StartCoroutine( SendGetRequesst("connection"));

        return false;
    }

    IEnumerator SendGetRequesst(String endpoint)
    {
        String url = "http://" + serverURL + "/" + endpoint;
        Debug.Log("sending web request to " + url);
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log("Get request send successfully");
                Debug.Log("Results: " + request.result);
                Debug.Log(request.downloadHandler.text);
            }
        }
    }
    void LoadState(BitArray bitState)
    {
        //TODO: maybe change the method signature to use a diffrent input
        //TODO: implement this, just use the forcePlayMove
    }
}
