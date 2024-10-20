using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.WebSockets;
using System.Threading;
using System.Text;
using TMPro;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net;

public class OnlineManager : MonoBehaviour
{
    public bool localhostoverride = false;

    string serverURL = "wss://server.ulttictactoe.com:8080";
    //string domain = "https://server.ulttictactoe.com";

    public bool canReachGoogle = false;
    public bool canReachServerHTTP = false;
    private string googleDNS = "8.8.8.8";
    public bool canReachServer = false;
    private ClientWebSocket ws;
    static GameManager gameManager;
    static BigGridManager bigGridManager;
    static GameObject smallGrids;

    public GameData loadedGame;

    public string uuid;
    string version = "0.0.1";

    // TODO: better method of doing this
    bool showMenus=true;
    public GameObject menuCanvas;
    // that will likely never happen


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

        Debug.Log("checking internet and server connection");
        checkInternetConnection();
        checkServerConnection();
    }


    // Update is called once per frame
    void Update()
    {
        menuCanvas.SetActive(showMenus);

        canReachServer = checkServerConnection();
        if (!canReachServer)
        {
            showMenus = true;
        }
        try
        {
            GameObject.Find("Text for the UUID - the UUID TEXT OBJECT").GetComponent<TextMeshProUGUI>().text = "UUID: " + uuid;
        }
        catch (Exception)
        {

            throw;
        }

    }

    public void makeNewConnectionWithServer()
    {
        try
        {
            //I do not know if this actually closes the connection
            ws.Abort();
        }
        catch
        {

        }
        Debug.Log("dropping old connection with server and making new one");
        ws = new ClientWebSocket();
        Connect();
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
            try
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Debug.Log("Message from server: " + message);
                    MessageHandler(message);
                    Array.Clear(buffer, 0, buffer.Length); // Clear buffer after processing
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error receiving message: " + ex.Message);
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
            case "ServerRejection":
                ServerRejectionHandler(message);
                break;
            case "Game":
                GameHandler(message);
                break;
            case "InitalConnection":
                InitalConnectionHandler(message);
                break;
            default:
                Debug.LogError("Server sent incorrectly formatted message");
                break;

                // Add other cases as needed
        }

    }

    private void GameHandler(KeyValuePair<string, string>[] message)
    {

        switch (message[0].Value) {

            case "Quickmatch":
                QuickmatchHandler(message);
                break;

            case "moveSentToThisUser":
                MoveSentToThisUserHandler(message);
                break;
            default:
                Debug.LogError("Server sent response that this is not able to handle");
                break;
        }
    }

    private void MoveSentToThisUserHandler(KeyValuePair<string, string>[] message)

    {

        Debug.Log("\n\n\nrecieved move!");
        GameData game = getGameDataFromMessage(message, 0);

        if (!game.gameId.Equals(loadedGame.gameId))
        {
            Debug.LogWarning("This client was sent a move for an unloaded game");
            return;
        }


        
        int lastMoveState =  game.gameState[game.lastMove];
        
        BoxClicked boxScript =  GameObject.Find("Small Grids").transform.GetChild(game.lastMove / 9).GetChild(2).GetChild(game.lastMove % 9).GetComponent<BoxClicked>();

        if (boxScript == null)
            Debug.Log("Failed to get box script for some reason");


        gameManager.audioManager.PlayClip("ClickSucceed");

        Debug.Log("running box scripts to simulate move");

        boxScript.SpawnXorO(lastMoveState==1);
        boxScript.UpdateScoreTracking(lastMoveState == 1);

        //update who's turn it is.
        gameManager.xPlayerTurn = !gameManager.xPlayerTurn;

        boxScript.MoveWhereToPlay();



        gameManager.canHumanPlayerPlay = true;
    }

    private void QuickmatchHandler(KeyValuePair<string, string>[] message)
    {
        Debug.Log("recieved quickmatch");

        GameData game = getGameDataFromMessage(message, 0);

        loadedGame = game;

        Debug.Log("set Loaded game");

        GameObject.Find("Canvas_Menus").GetComponent<Canvas>().enabled = false;

        Debug.Log("should have hidden canvas");

        //if you are player 0 and player 0 is allowed to play set true, if you are player 1 and player0toPlayNext false, also true, other combinations false
        gameManager.canHumanPlayerPlay = (game.player0toPlayNext == (game.uuid0 == uuid));
        gameManager.xPlayerTurn = game.player0toPlayNext;

    }

    GameData getGameDataFromMessage(KeyValuePair<string, string>[] message, int lastIndexRead)
    {


        //todo: check keys are valid or switch to using a dict
        return new GameData(message[1+ lastIndexRead].Value,
        message[2+lastIndexRead].Value,
        message[3+lastIndexRead].Value,
        ExtractIntegersFromString(message[4+lastIndexRead].Value),
        int.Parse(message[5+lastIndexRead].Value),
        int.Parse(message[6+lastIndexRead].Value) == 1);
        
    }
    private void ServerRejectionHandler(KeyValuePair<string, string>[] message)
    {
        Debug.Log("Sever Rejected message for reason: " + message[0].Value);
        for (int i = 1; i < message.Length; i++)
        {

            Debug.Log("Other information: " + message[i].Key + " : " + message[i].Value);
        }

    }

    private void InitalConnectionHandler(KeyValuePair<string, string>[] message)
    {


        string uuidFromServer = message[1].Value;

        uuid = uuidFromServer;


    }

    public void SendMessageToServer(string message)
    {

        Debug.Log("attempting to send message. " + message);
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
        TMP_InputField username = GameObject.Find("Username InputField (TMP)").transform.GetComponentInChildren<TMP_InputField>();
        TMP_InputField password = GameObject.Find("Password InputField (TMP) (1)").transform.GetComponentInChildren<TMP_InputField>();
        //   TextMeshPro status = GameObject.Find("Status").transform.GetComponentInChildren<TextMeshPro>();
        //     TextMeshPro token = GameObject.Find("Token DO NOT SHOW THE USER THIS EVER THEY ARE STUPID").transform.GetComponentInChildren<TextMeshPro>();


        var authData = initalData();
         authData.Add(   makePair("Auth", "RegisterPassword"));
         authData.Add( makePair("username", username.text)   );
        authData.Add(makePair("password", password.text));
        
        

        string jsonString = JsonConverter.ListToJson(authData);

        SendMessageToServer(jsonString);

    }

    public List<KeyValuePair<string, string>> initalData()
    {

        var data = new List<KeyValuePair<string, string>>
        {
            makePair("version", version),
            makePair("uuid", uuid),
        };
        return data;
    }

    public void JoinMatchQueue()
    {
        var requestData = new List<KeyValuePair<string, string>>
        {
            makePair("version", version),
            makePair("uuid", uuid),
            makePair("Game", "JoinMatchmaking")
        };

        string message = JsonConverter.ListToJson(requestData);

        SendMessageToServer(message);
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

    void checkInternetConnection()
    {
        StartCoroutine(CheckInternet());
    }

    IEnumerator CheckInternet()
    {
        // Send a ping to Google's DNS
        Ping ping = new Ping(googleDNS);

        // Wait until the ping returns or times out
        while (!ping.isDone)
        {
            yield return null;
        }

        // Check if the ping was successful
        if (ping.time >= 0)
        {
            Debug.Log("Internet is available! Reached Google.");
            canReachGoogle = true;
        }
        else
        {
            Debug.Log("No internet connection. Could not reach Google.");
            canReachGoogle = false;
        }


        // Send an HTTP request to ulttictactoe.com
        IPAddress[] addresses = Dns.GetHostAddresses(serverURL);

        if (addresses.Length > 0)
        {
            // Use the first resolved IP address to ping
            Ping pingTest = new Ping(addresses[0].ToString());
            // Wait for the ping to complete
            while (!pingTest.isDone)
            {
                yield return null;
            }

            if (pingTest.time >= 0)
            {
                Debug.Log("Ping to " + serverURL + " successful!");
                canReachServerHTTP = true;
            }
            else
            {
                Debug.Log("Ping to " + serverURL + " failed.");
                canReachServerHTTP = false;
            }
        }
    }

    bool checkServerConnection()
    {
        if (ws.State == WebSocketState.Open)
        {
            //Debug.Log("WebSocket status: open");
            return true;
        }
        else
        {
            //Debug.LogError("WebSocket status: not open");
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
    {//TODO: look at similar implementation in the recieved move handler to finish this thing (if we need to)
        if (gameManager.canHumanPlayerPlay)
        {
            Debug.LogWarning("forcing placement while player can play, are you sure that is what is supposed to be happening?");
        }

        Transform smallGridContainingLocation = smallGrids.transform.GetChild(location / 9);
        BoxClicked box = smallGridContainingLocation.GetChild(2).GetChild(location % 9).GetComponent<BoxClicked>();

        box.SpawnXorO(isX);
       // box.UpdateScoreTracking();

    }
    public KeyValuePair<string, string> makePair(string k, string v)
    {
        return new KeyValuePair<string, string>(k, v);
    }

    public static int[] ExtractIntegersFromString(string input)
    {
        // Regex pattern to match integers (including negative numbers)
        const string pattern = @"-?\d+";

        // Use LINQ to find all matches, convert them to integers, and return as array
        return Regex.Matches(input, pattern)
                    .Cast<Match>()
                    .Select(m => int.Parse(m.Value))
                    .ToArray();
    }

    internal void humanPlayerPlayedAt(Transform transform, bool playingAnX)
    {
        int locationPlayed ;
        int smallLocation = transform.GetSiblingIndex();
        int offset = transform.parent.parent.GetSiblingIndex() * 9;
        locationPlayed = smallLocation + offset;
        Debug.Log("telling server player played at location " + locationPlayed);
        var messageToServer = initalData();
        messageToServer.Add(makePair("Game", "MakeMove"));
        messageToServer.Add(makePair("gameid", loadedGame.gameId));
        messageToServer.Add(makePair("location", ""+locationPlayed));
        messageToServer.Add(makePair("played", (playingAnX ? "1" : "-1")));
        string jsonToServer = JsonConverter.ListToJson(messageToServer);
        Debug.Log("sendingThisToServer " + jsonToServer);
        SendMessageToServer(jsonToServer);


    }
}