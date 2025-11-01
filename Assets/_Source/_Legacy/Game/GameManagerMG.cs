using UnityEngine;
using System;
using Web.New.Messages.Payloads.ServerClient;

public class GameManagerMG : MonoBehaviour
{
    public GameObject controlButtons;

    [SerializeField] private PlayerStatsController player1;
    [SerializeField] private PlayerStatsController player2;
    [SerializeField] private PlayerStatsController player3;

    [SerializeField] public HandManager handManager;
    [SerializeField] private TableManager tableManager;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private BidManager bidManager;

    [SerializeField] private GameTimer timer;

    [SerializeField] private ParticleSystem bombParticle;
    [SerializeField] private ParticleSystem RocketParticle;

    [SerializeField] private GameObject winGamePopup;
    [SerializeField] private GameObject loseGamePopup;
    public event Action<int> OnBomb;

    private WebGameManager webGameManager;
    private int passCount = 0;
    private bool autoSetTurn = true;

    void Start()
    {
        if (!gameManager)
            gameManager = FindObjectOfType<GameManager>();
        webGameManager = FindObjectOfType<WebGameManager>();
        if (webGameManager)
        {
            webGameManager.OnSetTurnRecievedMG += HandleSetTurnMG;
            webGameManager.OnLandlord += HandleLandlord;
            webGameManager.OnGameRestart += HandleRestart;
            webGameManager.OnWinGame += HandleEndGame;
            if (webGameManager.isMultiplayer)
            {
                autoSetTurn = false;
            }
            webGameManager.eventRouter.OnPlayUpdate += HandlePlayUpdate;
            webGameManager.eventRouter.OnPassUpdate += HandlePassUpdate;
            webGameManager.eventRouter.OnPlayRequest += HandlePlayRequest;
            webGameManager.eventRouter.OnGameOver += HandleGameOverMessage;
        }
    }

    void HandleRestart()
    {
        controlButtons.SetActive(false);
    }

    private void HandleLandlord(long value, int val)
    {
        controlButtons.SetActive(false);
        if (gameManager.mainPlayer.playerId == value)
        {
            controlButtons.SetActive(true);
            timer.StartTimer();
        }
        else if (!webGameManager.isMultiplayer)
        {
            webGameManager.SendMessage(new SetTurnMessage(GameManager.GetNextPlayer(GameManager.GetNextPlayer(value)), new int[0], false));
        }
    }

    private void HandlePlayUpdate(PlayUpdatePayload message)
    {
        var newMsg = new SetTurnMessage(message.user_id, message.cards, false);
        gameManager.gameState = GameState.game;
        HandleSetTurnMG(newMsg);
    }

    private void HandlePassUpdate(PassUpdatePayload message)
    {
        var newMsg = new SetTurnMessage(message.user_id, new int[0], true);
        gameManager.gameState = GameState.game;
        HandleSetTurnMG(newMsg);
    }

    private void HandlePlayRequest(PlayRequestPayload message)
    {
        Debug.Log("GameManagerMG: принял запрос на ход. wgm id " + webGameManager.playerId + " msg id " + message.current_player);
        if (message.current_player == gameManager.mainPlayer.playerId || message.current_player == webGameManager.playerId || message.current_player == webGameManager.connectionManager.connectionProps.user_id)
        {
            if (message.current_player == message.turn_player)
            {
                if (!message.is_forced_turn)
                {
                    Debug.Log("GameManagerMG: проверка пройдена");
                    if (handManager.cardsInHand.Count > message.cards.Length)
                    {
                        handManager.Clear();
                        gameManager.DealHandler(new DealPayload(message.cards));
                    }
                    controlButtons.SetActive(true);
                    handManager.CheckTurn();
                    timer.StartTimer();
                }
                else
                {
                    MakeForcedTurn(message.turn_player, message.cards);
                }
            }
            else
            {
                MakeForcedTurn(message.turn_player, message.cards);
            }
        }
    }

    private void MakeForcedTurn(long playerId, int[] cards)
    {
        Card[] cardArray = gameManager.GetCardsByIndexes(cards);
        Card[] hintedCards;
        if (handManager.lastPlacedCards == null || handManager.lastPlacedCards.Length == 0)
        {
            hintedCards = ComboValidator.FindRandomCombo(cardArray);
        }
        else
        {
            hintedCards = ComboValidator.FindStrongerCombo(cardArray, handManager.lastPlacedCards);
        }

        if (hintedCards == null || hintedCards.Length == 0)
        {
            PassForPlayer(playerId);
        }
        else
        {
            int[] sendedCards = new int[hintedCards.Length];
            for (int i = 0; i < hintedCards.Length; i++)
                sendedCards[i] = hintedCards[i].numberInDeck;
            TurnForPlayer(playerId, sendedCards, ComboValidator.ValidateCombo(hintedCards));
        }
    }

    private void HandleSetTurnMG(SetTurnMessage message)
    {
        Debug.Log("GameManagerMG: вызван обработчик");
        if (gameManager.gameState == GameState.game)
        {
            controlButtons.SetActive(false);

            if (message.isPass == true)
            {
                passCount++;
                if (passCount >= 2)
                {
                    tableManager.Clear();
                    handManager.lastPlacedCards = new Card[0];
                }
            }
            else
            {
                handManager.lastPlacedCards = gameManager.GetCardsByIndexes(message.cardsPlaced);
                var comboType = ComboValidator.ValidateCombo(handManager.lastPlacedCards);
                if (comboType == ComboType.Bomb)
                {
                    bombParticle.Play();
                    OnBomb?.Invoke(1);
                }
                if (comboType == ComboType.Rocket)
                {
                    RocketParticle.Play();
                    OnBomb?.Invoke(1);
                }
                passCount = 0;
            }
            if (GameManager.GetNextPlayer(message.playerId) == gameManager.mainPlayer.playerId && autoSetTurn)
            {
                controlButtons.SetActive(true);
                handManager.CheckTurn();
                timer.StartTimer();
            }
        }
    }

    public void Turn(int[] cardsPlaced, ComboType comboType = ComboType.Invalid)
    {
        timer.StopTimer();
        TurnForPlayer(gameManager.mainPlayer.playerId, cardsPlaced, comboType);
        controlButtons.SetActive(false);
    }

    public void TurnForPlayer(long playerId, int[] cardsPlaced, ComboType comboType = ComboType.Invalid)
    {
        if (webGameManager)
        {
            webGameManager.SendMessage(new SetTurnMessage(playerId, cardsPlaced, false));
        }
        webGameManager.connectionManager.SendPlay(playerId, cardsPlaced, comboType);
    }

    public void Pass()
    {
        if (gameManager.gameState == GameState.game)
        {
            timer.StopTimer();
            PassForPlayer(gameManager.mainPlayer.playerId);
        }
    }

    public void PassForPlayer(long playerId)
    {
        if (gameManager.gameState == GameState.game)
        {
            webGameManager.SendMessage(new SetTurnMessage(playerId, new int[0], true));
        }
        webGameManager.connectionManager.SendPass(playerId);
    }

    public void SendWinGame()
    {
        if (webGameManager)
        {
            if (gameManager.mainPlayer.isLandlord)
                webGameManager.SendMessage(new WinMessage(1));
            if (!gameManager.mainPlayer.isLandlord)
                webGameManager.SendMessage(new WinMessage(0));
        }
    }

    public void HandleGameOverMessage(GameOverPayload message)
    {
        PlayerStatsController player = new PlayerStatsController();
        if (gameManager.mainPlayer.playerId == message.winner_id)
            player = gameManager.mainPlayer;
        if (gameManager.player2.playerId == message.winner_id)
            player = gameManager.player2;
        if (gameManager.player3.playerId == message.winner_id)
            player = gameManager.player3;

        int type = player.isLandlord ? 1 : 0;

        HandleEndGame(type);
    }

    public void HandleEndGame(int winnerType)
    {
        if ((gameManager.mainPlayer.isLandlord && winnerType == 1) || (!gameManager.mainPlayer.isLandlord && winnerType == 0))
        {
            WinGame();
        }
        else
        {
            LoseGame();
        }
    }

    private void WinGame()
    {
        winGamePopup.SetActive(true);
        int totalBalance = player1.pointsCount + bidManager.totalBid;
        WebglBridge.SendScoreUpdate(true, bidManager.totalBid, totalBalance);
        if (!webGameManager.isMultiplayer)
            webGameManager.connectionManager.SendUpdateSoftBalance(totalBalance);
    }

    private void LoseGame()
    {
        loseGamePopup.SetActive(true);
        int totalBalance = player1.pointsCount - bidManager.totalBid;
        WebglBridge.SendScoreUpdate(false, -bidManager.totalBid, totalBalance);
        if (!webGameManager.isMultiplayer)
            webGameManager.connectionManager.SendUpdateSoftBalance(totalBalance);
    }
}