using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.WebSockets;
using System.Threading;
using System.Text;
using TMPro;

public class OnlineManager : MonoBehaviour
{
    public bool localhostoverride = false;

    string serverURL = "wss://server.ulttictactoe.com:8080";

    public bool canReachGoogle = true;
    public bool canReachServer = true;
    private ClientWebSocket ws;
    static GameManager gameManager;
    static BigGridManager bigGridManager;
    static GameObject smallGrids;

    static readonly List<string> SERVERCOMMANDS = new List<string>{ "auth"};
 
    public  string uuid;
    string version = "0.0.1";

    private void Awake()
    {
        if (localhostoverride)
        {
            serverURL = "ws://localhost:8080";
        }

        

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.multiplayerEnabled = true;
        gameManager.canHumanPlayerPlay = false;


      


    }

    // Start is called before the first frame update
    void Start()
    {
      
        bigGridManager = GameObject.Find("BigGrid").GetComponent<BigGridManager>();

        smallGrids = GameObject.Find("Small Grids");

        ws = new ClientWebSocket();
        Connect();

        Debug.Log("checking server connection");
        checkServerConnection();
    }

    // Update is called once per frame
    void Update()
    {
        checkServerConnection();
    }

    async void Connect()
    {
        try
        {
            await ws.ConnectAsync(new Uri(serverURL), CancellationToken.None);
            Debug.Log("Connected to server");
            ReceiveMessages();
        }
        catch (Exception e)
        {
            Debug.LogError("Connection error: " + e.Message);
        }
    }

    async void ReceiveMessages()
    {
        var buffer = new byte[1024];
        while (ws.State == WebSocketState.Open)
        {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Debug.Log("Message from server: " + message);

                MessageHandler( message);
            }
        }
    }
    void MessageHandler(string json)
    {
     var message = JsonConverter.JsonToList(json).ToArray();
        


        switch (message[0].Key)
        {
            case "Auth":
                AuthHandler(message);
                // Handle the 'auth' case here
                break;
            case "InitalConnection":
                InitalConnectionHandler(message);
                break;

                // Add other cases as needed
        }

    }

    private void InitalConnectionHandler(KeyValuePair<string, string>[] message)
    {
        
        TextMeshPro uuidText = GameObject.Find("UUID").transform.GetChild(0).GetComponent<TextMeshPro>();
        string uuidFromServer = message[1].Value;

        uuid = uuidFromServer;

        Debug.Log("uuidfromserver: " + uuidFromServer);
        Debug.Log(uuidText == null);
        Debug.Log("uuidText: " + uuidText.text);
        uuidText.text = "UUID: " + uuidFromServer;
    }

    public void SendMessageToServer(string message)
    {
        if (ws.State == WebSocketState.Open)
        {
            var messageBuffer = Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(messageBuffer);

            ws.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Message sent successfully. message sent: " + message);
                }
                else
                {
                    Debug.LogError("Error sending message: " + task.Exception.Message);
                }
            });
        }
        else
        {
            Debug.LogError("WebSocket is not open.");
        }
    }


    public void AttemptPasswordRegistration()
    {
        TextMeshPro username = GameObject.Find("Username InputField (TMP)").transform.GetComponentInChildren<TextMeshPro>();
        TextMeshPro password = GameObject.Find("Password InputField (TMP) (1)").transform.GetComponentInChildren<TextMeshPro>();
        //   TextMeshPro status = GameObject.Find("Status").transform.GetComponentInChildren<TextMeshPro>();
        //     TextMeshPro token = GameObject.Find("Token DO NOT SHOW THE USER THIS EVER THEY ARE STUPID").transform.GetComponentInChildren<TextMeshPro>();


        var authData = new List<KeyValuePair<string, string>>
        {
            makePair("version", version),
            makePair("uuid", uuid),
            makePair("Auth", "RegisterPassword"),
            makePair("username", username.text),
            makePair("password", password.text)
        };

        string jsonString = JsonConverter.ListToJson(authData);
        SendMessageToServer(jsonString);

    }

    public void AuthHandler(KeyValuePair<string, string>[] message)
    {
        switch (message[0].Value)
        {
            case "RegisterPasswordFailed":
                // Handle register password failure
                break;
            case "RegisterPasswordSuccess":
                // Handle register password success
                break;
            case "LoginSuccess":
                // Handle successful login
                break;
            case "LoginFailed":
                // Handle failed login
                break;
            case "TokenExpired":
                // Handle expired token
                break;
            case "UsernameAlreadyTaken":
                // Handle password change requirement
                break;
        }
    }

    bool checkInternetConnection()
    {
        //TODO: ping Google or something
        return false;
    }

    bool checkServerConnection()
    {
        if (ws.State == WebSocketState.Open)
        {
            //   Debug.Log("WebSocket status: open");
            return true;
        }
        else
        {
            //  Debug.LogError("WebSocket status: not open");
            return false;
        }
    }

    void LoadState(BitArray bitState)
    {
        //TODO: maybe change the method signature to use a different input
        //TODO: implement this, just use the forcePlayMove
    }

    private void OnDestroy()
    {
        if (ws != null)
        {
            ws.Dispose();
        }
    }

    void ForcePlace(byte location, bool isX)
    {
        if (gameManager.canHumanPlayerPlay)
        {
            Debug.LogWarning("forcing placement while player can play, are you sure that is what is supposed to be happening?");
        }

        Transform smallGridContainingLocation = smallGrids.transform.GetChild(location / 9);
        BoxClicked box = smallGridContainingLocation.GetChild(2).GetChild(location % 9).GetComponent<BoxClicked>();

        box.SpawnXorO(isX);
        box.UpdateScoreTracking();

    }
    public KeyValuePair<string, string> makePair(string k, string v)
    {
        return new KeyValuePair<string, string>(k, v);
    }
    
}