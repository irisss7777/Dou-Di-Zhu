using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;
using System.Collections;
using UnityEngine.SceneManagement;
using Web.New.Connection.Client;
using Web.New.Connection.Router;
using Unity.VisualScripting;
using Web.New.Messages.Payloads.ServerClient;
using Web.New.Messages.Payloads.ClientServer;

public class WebGameManager : MonoBehaviour
{
#region Public
    public ConnectionManager connectionManager;
    public GameEventRouter eventRouter;
    public string roomName = "";
    public UserStatsContainer userStats;
    public long playerId = 0;
    public long roomId = 0;
    public int gameMode = 0;
    public int roomCurrencyType = 2;
    public bool gameStarted = false;
    public PlayerStatsMessage webObjectStats;
    public bool isMultiplayer = true;
    public bool isTutorial = false;
    public bool isTestInUnity = false;
    public bool isTestGameStateMessage = false;
#endregion

    #region Private

    #endregion

    #region Actions
    public event Action OnGameStart;
    public event Action OnGameRestart;
    public event Action<DeckMessage> OnGetDeck;
    public event Action OnOpponentLost;
    public event Action<PlayerStatsMessage> OnPlayerStatsRecieved;
    public event Action<SetTurnMessage> OnSetTurnRecievedLLS;
    public event Action<SetTurnMessage> OnSetTurnRecievedMG;
    public event Action<EmojiMessage> OnEmojiRecieved;
    public event Action<long,int> OnLandlord;
    public event Action<int> OnWinGame;

    #endregion

    async void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        //WebglBridge.SendLoadedEvent();
        //webObjectStats = new PlayerStatsMessage();
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (isTestInUnity)
        {
            SetSinglePlayer();
        }
        if (eventRouter == null)
        {
            Debug.Log("EventRouter не указан в WebGameManager. Игровые события обрабатываться не смогут!");
        }
        else
        {
            eventRouter.OnGamePrepare += PrepareToGame;
            eventRouter.OnGameStart += InvokeLandlordEvents;
            eventRouter.OnConnectedToLobby += (ConnectedPayload message) => { roomId = message.lobby_id; connectionManager.connectionProps.lobby_id = message.lobby_id; };///////////
        }
        if (isTestGameStateMessage)
        {
            StartCoroutine(TEST());
            StartCoroutine(FindObjectOfType<GameEventRouter>().TEST());
        }
        
    }

    private IEnumerator TEST()
    {
        yield return new WaitForSeconds(2);
        FindObjectOfType<GameManager>().mainPlayer.playerId = 7;
        playerId = 7;
        connectionManager.connectionProps.user_id = 7;
    }

    private void InvokeLandlordEvents(GameStartPayload message)
    {
        Debug.Log("WebGameManager: вызов множества событий.");
        OnLandlord?.Invoke(message.landlord_id, message.final_bid);
    }

    public void SetSinglePlayer()
    {
        isMultiplayer = false;
        StartMessage msg = new StartMessage();
        msg.playerId = 1;
        SendMessage(msg);
    }

    public void GameStartHandler() //При запуске клиента из React
    {
        if (isMultiplayer)
        {
            //Connect
            if (connectionManager != null)
            {
                connectionManager.Initialize();
                connectionManager.StartGameConnection(new ConnectPayload(roomId, playerId, roomName, userStats.nickname, userStats.points, gameMode, roomCurrencyType, userStats.roomStartBid, userStats.roomMaxBid));
            }
            else
            {
                Debug.Log("ConnectionManager не указан в WebGameManager.");
            }
        }
    }

    private void PrepareToGame(object obj)
    {
        OnGameStart?.Invoke();
    }

    void HandleServerMessage(string message)
    {
        try
        {
            var data = JsonUtility.FromJson<Message>(message);

            switch (data.type)
            { 
                case "start":
                    var startMessage = JsonUtility.FromJson<StartMessage>(message);
                    playerId = startMessage.playerId;
                    Debug.Log("Start Invoking");
                    OnGameStart?.Invoke();
                    //WebglBridge.SendStartEvent();         
                    gameStarted = true;
                    break;

                case "restart":
                    var restartMessage = JsonUtility.FromJson<RestartMessage>(message);
                    OnGameRestart?.Invoke();
                    gameStarted = true;
                    break;               

                case "player_stats":
                    var playerStatsMessage = JsonUtility.FromJson<PlayerStatsMessage>(message);
                    OnPlayerStatsRecieved?.Invoke(playerStatsMessage);
                    break;

                case "deck":
                    var deckMessage = JsonUtility.FromJson<DeckMessage>(message);
                    OnGetDeck?.Invoke(deckMessage);
                    break;

                case "emoji":
                    var emojiMessage = JsonUtility.FromJson<EmojiMessage>(message);
                    OnEmojiRecieved?.Invoke(emojiMessage);
                    break;
                
                case "landlord":
                    var landlordMessage = JsonUtility.FromJson<LandlordMessage>(message);
                    OnLandlord?.Invoke(landlordMessage.playerId,landlordMessage.bid);
                    break;
                
                case "win":
                    var winMessage = JsonUtility.FromJson<WinMessage>(message);
                    OnWinGame?.Invoke(winMessage.playerId);
                    break;

                case "set_turn":
                    var setTurnMessage = JsonUtility.FromJson<SetTurnMessage>(message);
                    if(setTurnMessage.isMainGame == false)
                        OnSetTurnRecievedLLS?.Invoke(setTurnMessage);
                    if(setTurnMessage.isMainGame == true)
                        OnSetTurnRecievedMG?.Invoke(setTurnMessage);
                    break;
                
                case "player_left":
                    OnOpponentLost?.Invoke();
                    Debug.Log("Игрок покинул комнату.");
                    break;

                case "timed":
                    break;
            }
        }
        catch (Exception ex)
        {
            if(isMultiplayer)
                Debug.LogError("Ошибка при обработке сообщения: " + ex.Message + " " + message);
        }
    }

    public void SendMessage(object obj)
    {
        string json = JsonUtility.ToJson(obj);
        //if (ws.State == WebSocketState.Open)
        //{
        //    ws.SendText(json);
        //}
        //else
        //{
        //    Debug.LogWarning("WebSocket не открыт. Сообщение не отправлено.");
        //}
        if(!isMultiplayer)
        {
            HandleServerMessage(json);
        }
    }


#region Message Senders
    public void SendObjStats()
    {;
        SendMessage(webObjectStats);
        //WebglBridge.SendLoadedEvent();
    }
#endregion

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            Destroy(gameObject);
        }
    }

    async void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        WebglBridge.SendExitEvent();
        if (eventRouter)
        {
            eventRouter.OnGamePrepare -= PrepareToGame;
            eventRouter.OnGameStart -= InvokeLandlordEvents;
        }
    }
}