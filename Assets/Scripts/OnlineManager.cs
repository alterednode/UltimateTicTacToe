using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Linq;
using NativeWebSocket;
using UnityEditor;
using UnityEngine.XR;

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

    public string username;
    public string uuid;
    private string sessionToken;
    private string refreshToken;
    string version = "0.0.2";

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
        var message = JsonConverter.JsonToDictionary(json);
        

        switch (message["responseType"])
        {
            case "auth":
                AuthHandler(message);
                // Handle the 'auth' case here
                break;
            case "serverRejection":
                ServerRejectionHandler(message);
                break;
            case "game":
                GameHandler(message);
                break;
            case "initalConnection":
                InitalConnectionHandler(message);
                break;
            case "status":
                throw new NotImplementedException();
                //break; no need to break if throwing error
            default:
                Debug.LogError("Server sent incorrectly formatted message");
                break;

                // Add other cases as needed
        }

    }

    private void GameHandler(Dictionary<string, string> message)
    {

        switch (message["gameResponse"])
        {

            case "quickmatchStarted":
                QuickmatchHandler(message);
                break;

            case "moveSentToThisUser":
                MoveSentToThisUserHandler(message);
                break;
            case "joinedMatchQueue":
                //don't really need to do anythign
                break;
            default:
                Debug.LogError("Server sent response that this is not able to handle: " + message["gameResponse"]);
                break;
        }
    }

    private void MoveSentToThisUserHandler(Dictionary<string, string> message)

    {

        Debug.Log("\n\n\nrecieved move!");
        GameData game = getGameDataFromMessage(message);

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

    private void QuickmatchHandler(Dictionary<string, string> message)
    {
        Debug.Log("recieved quickmatch");

        GameData game = getGameDataFromMessage(message);

        loadedGame = game;

        Debug.Log("set Loaded game");
        
        menuCanvas.SetActive(false);

        Debug.Log("should have hidden mult menu canvas");

        //if you are player 0 and player 0 is allowed to play set true, if you are player 1 and player0toPlayNext false, also true, other combinations false
        gameManager.canHumanPlayerPlay = (game.player0toPlayNext == (game.uuid0 == uuid));
        gameManager.xPlayerTurn = game.player0toPlayNext;

    }

    GameData getGameDataFromMessage(Dictionary<string, string> message)
    {
        return new GameData(
            message["gameID"],
            message["player0"],
            message["player1"],
            ExtractIntegersFromString(message["gameState"]),
            int.Parse(message["lastMove"]),
            message["player0toPlayNext"].Equals("1")
        ); ;
    }
    private void ServerRejectionHandler(Dictionary<string, string> message)
    {

        if (message.TryGetValue("reason", out string reasonForServerRejection))
        {
            Debug.Log("Sever Rejected message for reason: " + reasonForServerRejection);

            if (reasonForServerRejection.Equals("invalidSessionToken") || reasonForServerRejection.Equals("expiredSessionToken"))
            {
                //TODO: try and reauthenticate with token.
            }
        }
        else
        {
            Debug.Log("no reason provided for server rejection, listing all information provided by server");
            foreach (var kvp in message)
            {
                Debug.Log("Information: " + kvp.Key + " : " + kvp.Value);
            }

        }


    }

    private void InitalConnectionHandler(Dictionary<string, string> message)
    {
        string uuidFromServer = message["UUID"];

        uuid = uuidFromServer;
    }

    public async void SendMessageToServer(string message)
    {
        Debug.Log("Attempting to send message: " + message);

        if (ws.State == WebSocketState.Open)
        {
            await ws.SendText(message);
        }
        else
        {
            Debug.LogError("WebSocket is not open.");
        }
    }

    public void SendMessageToServer(Dictionary<string, string> dictMessage)
    {
        // Converts the dictionary to JSON and then sends the message
        SendMessageToServer(JsonConverter.DictionaryToJson(dictMessage));
    }




    public void AuthenticateUsernameAndPassword()
    {
        TMP_InputField username = GameObject.Find("Username InputField (TMP)").transform.GetComponentInChildren<TMP_InputField>();
        TMP_InputField password = GameObject.Find("Password InputField (TMP) (1)").transform.GetComponentInChildren<TMP_InputField>();

        var authData = initalData();
       
        authData.Add("messageType", "authenticateUsernameAndPassword");
        authData.Add("username", username.text);
        authData.Add("password", password.text);


        SendMessageToServer(authData);

    }
    

    public Dictionary<string, string> initalData()
    {

        var data = new Dictionary<string, string>();

        data.Add("version", version);
        data.Add("uuid", uuid);
        
        return data;
    }

    public void MatchButtonPressed()
    {

        if (opponentUsernameThing.text.Length!=0)
        {
            RequestToInviteUserToGame(opponentUsernameThing.text);
            setStatus("attempting to send match request to " +  opponentUsernameThing.text);

        }else if (inMatchmaking)
        {
            RequestToLeaveMatchmaking();
            matchmakingButton.text = "Enter Matchmaking";
            setStatus("Left Matchmaking queue");
        }
        else
        {
            RequestToJoinMatchmaking();
            setStatus("Joined Matchmaking queue");
            matchmakingButton.text = "Leave Matchmaking";            
        }



    }

    public void RequestToInviteUserToGame(String username) 
    {
        throw new NotImplementedException();
    }

    public void RequestToJoinMatchmaking()
    {
        inMatchmaking = true;
        Dictionary<string, string> requestData = new Dictionary<string, string>();
            
           requestData.Add("version", version);
          requestData.Add("uuid", uuid);
       requestData.Add("messageType", "game");
          requestData.Add("gameMessageType", "joinMatchmaking");
        requestData.Add("sessionToken", sessionToken);
            


        SendMessageToServer(requestData);
    }

    public void RequestToLeaveMatchmaking()
    {
        inMatchmaking = false;


        Dictionary<string, string> requestData = new Dictionary<string, string>();

        requestData.Add("version", version);
        requestData.Add("uuid", uuid);
        requestData.Add("messageType", "game");
        requestData.Add("gameMessageType", "leaveMatchmaking");
        requestData.Add("sessionToken", sessionToken);



        SendMessageToServer(requestData);
    }

    public void AuthHandler(Dictionary<string, string> message)
    {

        string authStatus = message["authStatus"];

        switch (authStatus)
        {
            case "success":
                Debug.Log("auth successful");
                if (message.TryGetValue("uuid", out var uuidFromServer))
                {
                    Debug.Log("Assigned new uuid");
                    uuid = uuidFromServer;
                    PlayerPrefs.SetString("uuid", uuidFromServer);
                }
                if (message.TryGetValue("refreshToken", out var refreshTokenFromServer))
                {
                    Debug.Log("new refreshToken");
                    refreshToken = refreshTokenFromServer;
                    PlayerPrefs.SetString("refreshToken", refreshTokenFromServer);
                }
                if (message.TryGetValue("sessionToken", out var sessionTokenFromServer))
                {
                    Debug.Log("new session token - not storing permenantly");
                    sessionToken = sessionTokenFromServer;
                }
                return;
            case "failure":
                if(message.TryGetValue("reason", out var reasonFromServer))
                {
                    switch (reasonFromServer)
                    {
                        case "expiredRefreshToken":
                        case "invalidRefreshToken":
                            ClearStoredTokenAndUUIDAndReload();
                            return;
                    }
                }
                return;
            case "logoutSuccess":
                ClearStoredTokenAndUUIDAndReload();
                //currently nothing to do if this happens
                return;



        }
    }

    void ClearStoredTokenAndUUIDAndReload()
    {

            sessionToken = null;
            refreshToken = null;
            PlayerPrefs.DeleteKey("refreshToken");
            PlayerPrefs.DeleteKey("uuid");

            SceneLoader.loadOnlineMultiplayer();
            return;
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
        messageToServer.Add("messageType", "game");
        messageToServer.Add("gameMessageType", "makeMove");
        messageToServer.Add("sessionToken",sessionToken);
        messageToServer.Add("gameid", loadedGame.gameid);
        messageToServer.Add("location", "" + locationPlayed);
        messageToServer.Add("played", (playingAnX ? "1" : "-1"));
        Debug.Log("sendingThisToServer " + JsonConverter.DictionaryToJson( messageToServer));
        SendMessageToServer(messageToServer);


    }
}