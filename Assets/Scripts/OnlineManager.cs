using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

public class OnlineManager : MonoBehaviour
{
    public bool localhostoverride = false;

    string serverURL = "ws://150.230.36.239:8080";

    public Boolean canReachServer = true;
    private ClientWebSocket ws;

    private void Awake()
    {
        if (localhostoverride)
        {
            serverURL = "ws://localhost:8080";
        }

        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.multiplayerEnabled = true;
        gameManager.canHumanPlayerPlay = false;

        ws = new ClientWebSocket();
        Connect();
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
            }
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
            Debug.Log("WebSocket status: open");
            return true;
        }
        else
        {
            Debug.LogError("WebSocket status: not open");
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
}