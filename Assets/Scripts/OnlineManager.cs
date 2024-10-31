using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Linq;
using NativeWebSocket;
using UnityEditor;

public class OnlineManager : MonoBehaviour
{
    public bool localhostoverride = false;

    string serverURL = "wss://server.ulttictactoe.com:8080";
    //string domain = "https://server.ulttictactoe.com";

    public bool canReachGoogle = false;
    public bool canReachServerHTTP = false;
    //private string googleDNS = "8.8.8.8";
    public bool canReachServer = false;
    private WebSocket ws;
    static GameManager gameManager;
    static GameObject smallGrids;

    public GameData loadedGame;

    public string uuid;
    string version = "0.0.1";

    // TODO: better method of doing this
    bool showMenus = true;
    public GameObject menuCanvas;
    // that will likely never happen

    public bool inMatchmaking = false;

    public TextMeshProUGUI status;

    public TMP_InputField opponentUsernameThing;

    public TextMeshProUGUI matchmakingButton;



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

        smallGrids = GameObject.Find("Small Grids");
        status = GameObject.Find("Status").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Connect();
        

    }


    // Update is called once per frame
    void Update()
    {

        #if !UNITY_WEBGL || UNITY_EDITOR
        ws.DispatchMessageQueue();
#endif

        canReachServer  =  checkServerConnection();


        if (!canReachServer)
        {
            menuCanvas.SetActive(true);
        }

        string buttonTextForInviting = "Send Game Invite";
        if (opponentUsernameThing.text.Length!=0)
        {
            if (inMatchmaking)
            {
                RequestToLeaveMatchmaking();
                inMatchmaking = false ;
                setStatus("left matchmaking");
            }
            matchmakingButton.text = buttonTextForInviting;

        }
        else
        {
            //checks if the buttonText was just set to the invite text but all text was removed from the username box
            if(matchmakingButton.text.Equals(buttonTextForInviting)) {
                matchmakingButton.text = "Join Matchmaking";
            }
        }
        
    }

    public void makeNewConnectionWithServer()
    {
        try
        {
            //TODO: figure out how to close the connection properly
            ws.Close();
        }
        catch
        {

        }
        Debug.Log("dropping old connection with server and making new one");
        Connect();
    }


    void setStatus(string text)
    {
        status.text = text;
    }


    async void Connect()
    {
        try
        {
            ws = new WebSocket(serverURL);

            ws.OnOpen += () =>
            {
                Debug.Log("Connection open!");
               setStatus( "Connected to server");
            };

            ws.OnError += (e) =>
            {
                Debug.Log("Error! " + e);
                setStatus("unknown websocket error");
            };

            ws.OnClose += (e) =>
            {

                Debug.Log("Connection closed!");
                setStatus("connecting . . .");
                StartCoroutine(ReconnectAfterDelay(1));
            };

            ws.OnMessage += (bytes) =>
            {
                // getting the message as a string
                var message = System.Text.Encoding.UTF8.GetString(bytes);
                Debug.Log("OnMessage! " + message);
                MessageHandler(message);
            };


            // waiting for messages
            await ws.Connect();

        }
        catch (Exception e)
        {
            Debug.LogError("Connection error: " + e.Message);
        }
    }

    private IEnumerator ReconnectAfterDelay(int delay)
    {
        yield return new WaitForSeconds(delay);
        Connect();
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
            case "Status":
                throw new NotImplementedException();
                //break; no need to break if throwing error
            default:
                Debug.LogError("Server sent incorrectly formatted message");
                break;

                // Add other cases as needed
        }

    }

    private void GameHandler(KeyValuePair<string, string>[] message)
    {

        switch (message[0].Value)
        {

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

        if (!game.gameid.Equals(loadedGame.gameid))
        {
            Debug.LogWarning("This client was sent a move for an unloaded game");
            return;
        }



        int lastMoveState = game.gameState[game.lastMove];

        BoxClicked boxScript = GameObject.Find("Small Grids").transform.GetChild(game.lastMove / 9).GetChild(2).GetChild(game.lastMove % 9).GetComponent<BoxClicked>();

        if (boxScript == null)
            Debug.Log("Failed to get box script for some reason");


        gameManager.audioManager.PlayClip("MultiplayerPlay");

        Debug.Log("running box scripts to simulate move");

        boxScript.SpawnXorO(lastMoveState == 1);
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
        
        menuCanvas.SetActive(false);

        Debug.Log("should have hidden mult menu canvas");

        //if you are player 0 and player 0 is allowed to play set true, if you are player 1 and player0toPlayNext false, also true, other combinations false
        gameManager.canHumanPlayerPlay = (game.player0toPlayNext == (game.uuid0 == uuid));
        gameManager.xPlayerTurn = game.player0toPlayNext;

    }

    GameData getGameDataFromMessage(KeyValuePair<string, string>[] message, int lastIndexRead)
    {

        //todo: check keys are valid or switch to using a dict
        return new GameData(message[1 + lastIndexRead].Value,
        message[2 + lastIndexRead].Value,
        message[3 + lastIndexRead].Value,
        ExtractIntegersFromString(message[4 + lastIndexRead].Value),
        int.Parse(message[5 + lastIndexRead].Value),
        int.Parse(message[6 + lastIndexRead].Value) == 1);

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

    public async void SendMessageToServer(string message)
    {

        Debug.Log("attempting to send message. " + message);
        if (ws.State == WebSocketState.Open)
        {
            await ws.SendText(message);
        }
        else
        {
            Debug.LogError("WebSocket is not open.");
        }
    }



    public void AttemptSignUp()
    {
        TMP_InputField username = GameObject.Find("Username InputField (TMP)").transform.GetComponentInChildren<TMP_InputField>();
        TMP_InputField password = GameObject.Find("Password InputField (TMP) (1)").transform.GetComponentInChildren<TMP_InputField>();

        var authData = initalData();
        authData.Add(makePair("messageType", "Auth"));
        authData.Add(makePair("authType", "SignUp"));
        authData.Add(makePair("username", username.text));
        authData.Add(makePair("password", password.text));

        string jsonString = JsonConverter.ListToJson(authData);

        SendMessageToServer(jsonString);

    }

    public void AttemptLogin()
    {
        TMP_InputField username = GameObject.Find("Username InputField (TMP)").transform.GetComponentInChildren<TMP_InputField>();
        TMP_InputField password = GameObject.Find("Password InputField (TMP) (1)").transform.GetComponentInChildren<TMP_InputField>();

        var authData = initalData();
        authData.Add(makePair("messageType", "Auth"));
        authData.Add(makePair("authType", "Login"));
        authData.Add(makePair("username", username.text));
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

    public void MatchButtonPressed()
    {
        List<KeyValuePair<string, string>> requestData =null;
        TextMeshProUGUI text = GameObject.Find("Matchmaking button").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (text == null)
        {
            Debug.LogError("Could not find Matchmaking button");
        }
        if (inMatchmaking)
        {

            requestData = new List<KeyValuePair<string, string>>
            {
            makePair("version", version),
            makePair("uuid", uuid),
            makePair("messageType","Game"),
            makePair("gameType", "LeaveMatchmaking")
            };

            text.text = "Enter Matchmaking";
            setStatus("Left Matchmaking queue");

        }
        else
        {

            requestData = new List<KeyValuePair<string, string>>
            {
            makePair("version", version),
            makePair("uuid", uuid),
            makePair("messageType","Game"),
            makePair("gameType", "JoinMatchmaking")
            };


            setStatus("Joined Matchmaking queue");
            text.text = "Leave Matchmaking";
        }


        inMatchmaking = !inMatchmaking;
        string message = JsonConverter.ListToJson(requestData);

        SendMessageToServer(message);
    }

    public void AuthHandler(KeyValuePair<string, string>[] message)
    {
        switch (message[0].Value)
        {
            case "RegisterPasswordFailed":
                setStatus("Failed to register password");
                // Handle register password failure
                break;
            case "RegisterPasswordSuccess":
                setStatus("Registered password");
                // Handle register password success
                break;
            case "LoginSuccess":
                uuid = message[1].Value;
                setStatus("Login Successful");
                // Handle successful login
                break;
            case "LoginFailed":
                setStatus("Login failed");
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


    private void OnDestroy()
    {
        if (ws != null)
        {
            ws.CancelConnection();
        }
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
        int locationPlayed;
        int smallLocation = transform.GetSiblingIndex();
        int offset = transform.parent.parent.GetSiblingIndex() * 9;
        locationPlayed = smallLocation + offset;
        Debug.Log("telling server player played at location " + locationPlayed);
        var messageToServer = initalData();
        messageToServer.Add(makePair("messageType", "Game"));
        messageToServer.Add(makePair("gameType", "MakeMove"));
        messageToServer.Add(makePair("gameid", loadedGame.gameid));
        messageToServer.Add(makePair("location", "" + locationPlayed));
        messageToServer.Add(makePair("played", (playingAnX ? "1" : "-1")));
        string jsonToServer = JsonConverter.ListToJson(messageToServer);
        Debug.Log("sendingThisToServer " + jsonToServer);
        SendMessageToServer(jsonToServer);


    }
}