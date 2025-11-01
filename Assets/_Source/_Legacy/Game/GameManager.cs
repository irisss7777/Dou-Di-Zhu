using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using Web.New.Messages.Payloads.ServerClient;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

public class GameManager : MonoBehaviour
{
    public PlayerStatsController mainPlayer;
    public PlayerStatsController player2;
    public PlayerStatsController player3;

    public AIPlayer bot2;
    public AIPlayer bot3;

    public HandManager handManager;
    public TableManager tableManager;
    public List<Card> cards;
    public GameObject controlButtons;
    public TextMeshProUGUI landlordChooseButtonText;
    public int startingHandSize = 17;
    public float drawDelay = 0.3f;
    public GameState gameState = GameState.chooseLandlord;
    private WebGameManager webGameManager;
    [SerializeField] private List<Card> currentDeck = new List<Card>();
    private List<Card> landlordDeck = new List<Card>();

    [SerializeField] private Card cardOnTable1;
    [SerializeField] private Card cardOnTable2;
    [SerializeField] private Card cardOnTable3;
    [SerializeField] private ParticleSystem springParticle;
    [SerializeField] private ParticleSystem robberyParticle;

    void Start()
    {
        webGameManager = FindObjectOfType<WebGameManager>();
        if (webGameManager)
        {
            webGameManager.OnOpponentLost += () =>
            {
                //SceneManager.LoadScene(0); 
            };
            webGameManager.OnGameRestart += () => { Debug.Log("RESTART!"); gameState = GameState.chooseLandlord; tableManager.Clear(); handManager.Clear(); StartCoroutine(StartGame()); };
            if (!webGameManager.isMultiplayer)
                webGameManager.OnGetDeck += DeckHandler;
            webGameManager.eventRouter.OnDealCards += DealHandler;
            webGameManager.OnLandlord += HandleLandlord;
            webGameManager.eventRouter.OnGameStart += AssignCardsOnTable;
            webGameManager.eventRouter.OnGameState += HandleGameState;
            webGameManager.eventRouter.OnRobbery += (RobberyPayload msg) => { robberyParticle.Play(); };
            webGameManager.eventRouter.OnSpring += (SpringPayload msg) => { springParticle.Play(); };

            SendPlayerReady();
            if (!webGameManager.isMultiplayer)
                StartCoroutine(StartGame());
        }
        else
        {
            int[][] deck = RandomDecksGenerator.GetShuffledDeck();
            foreach (int card in deck[0])
            {
                currentDeck.Add(cards[card]);
                Debug.Log(card);
            }
            //foreach(int card in deck[1])
            //{
            //    currentDeck.Add(cards[card]);
            //    Debug.Log(card);
            //}
            //foreach(int card in deck[2])
            //{
            //    currentDeck.Add(cards[card]);
            //    Debug.Log(card);
            //}
            currentDeck = Card.SortCardsByValue(currentDeck);
            landlordDeck.Clear();
            foreach (int card in deck[3])
                landlordDeck.Add(cards[card]);
            landlordDeck = Card.SortCardsByValue(landlordDeck);

            //cardOnTable1.AssignView(landlordDeck[0].cardView);
            //cardOnTable2.AssignView(landlordDeck[1].cardView);
            //cardOnTable3.AssignView(landlordDeck[2].cardView);

            StartCoroutine(DrawCards(17, false));
            //StartCoroutine(DrawCards(3,true));
        }
    }

    private void SendPlayerReady()
    {
        if (webGameManager.isMultiplayer)
        {
            webGameManager.connectionManager.SendPlayerReady();
        }
    }

    private IEnumerator StartGame()
    {
        if (webGameManager.playerId == 1)
        {
            yield return new WaitForSeconds(0.1f);
            webGameManager.SendMessage(new DeckMessage(webGameManager.gameMode));
            yield return new WaitForSeconds(3f);
            webGameManager.SendMessage(new SetTurnMessage(3, 0, 0, false));//Init first turn
        }
    }

    private void DeckHandler(DeckMessage message)
    {
        int[] playerDeck = new int[startingHandSize];
        switch (webGameManager.playerId)
        {
            case 1:
                playerDeck = message.player1_deck;
                break;
            case 2:
                playerDeck = message.player2_deck;
                break;
            case 3:
                playerDeck = message.player3_deck;
                break;
        }

        bot2.Deck = message.player2_deck;
        bot3.Deck = message.player3_deck;
        bot2.landlordCards = bot3.landlordCards = message.landlord_deck;
        foreach (int card in playerDeck)
            currentDeck.Add(cards[card]);
        currentDeck = Card.SortCardsByValue(currentDeck);
        foreach (int card in message.landlord_deck)
            landlordDeck.Add(cards[card]);
        landlordDeck = Card.SortCardsByValue(landlordDeck);
        StartCoroutine(DrawCards(startingHandSize, false));
    }


    #region Deal cards
    public void DealHandler(DealPayload message)
    {

        int[] playerDeck = message.cards;
        foreach (int card in playerDeck)
            if (!currentDeck.Contains(cards[card]))
                currentDeck.Add(cards[card]);
        currentDeck = Card.SortCardsByValue(currentDeck);
        //foreach(int card in message.landlord_deck)
        //    landlordDeck.Add(cards[card]);
        //landlordDeck = Card.SortCardsByValue(landlordDeck);
        StartCoroutine(DrawCards(playerDeck.Length, false));
    }

    public IEnumerator DrawCards(int count, bool sortOnEnd)
    {
        for (int i = 0; i < count; i++)
        {
            if (currentDeck.Count > 0 || landlordDeck.Count > 0)
            {
                AddCardToHand(TakeCard());
                yield return new WaitForSeconds(drawDelay);
            }
        }
        if (sortOnEnd)
            handManager.SortHand();
    }

    Card TakeCard()
    {
        if (currentDeck.Count > 0)
        {
            int position = 0;
            Card randomCard = currentDeck[0];
            currentDeck.RemoveAt(position);
            return randomCard;
        }
        else if (landlordDeck.Count > 0)
        {
            int position = 0;
            Card randomCard = landlordDeck[position];
            landlordDeck.RemoveAt(position);
            return randomCard;
        }
        return null;
    }

    void AddCardToHand(Card card)
    {
        if (card != null)
        {
            handManager.AddCard(card);
        }
    }

    #endregion

    private void HandleGameState(GameStatePayload message)
    {
        handManager.Clear();

        //Присвоение данных пользователей
        mainPlayer.playerId = webGameManager.connectionManager.connectionProps.user_id;
        Debug.Log($" Handle Game data. Main player is: {mainPlayer.playerId}, ");
        List<long> ids = new List<long>
        {
            message.players[0].user_id,
            message.players[1].user_id,
            message.players[2].user_id
        };
        ids.Remove(mainPlayer.playerId);
        player2.playerId = ids[0];
        player3.playerId = ids[1];
        Debug.Log($"Player 2 is: {player2.playerId}");
        Debug.Log($"Player 3 is: {player3.playerId}");
        foreach (Player player in message.players)
        {
            mainPlayer.SetPlayerStats(player);
            player2.SetPlayerStats(player);
            player3.SetPlayerStats(player);
        }

        //Раздача карт
        foreach (Player player in message.players)
        {
            if (player.user_id == mainPlayer.playerId)
            {
                if (player.cards != null && player.cards.Length > 0)
                    DealHandler(new DealPayload(player.cards));

                //Установка ставки
                var bidManager = FindObjectOfType<BidManager>();
                if (bidManager)
                {
                    bidManager.startBid = message.start_bet;
                    bidManager.UpdateValuesManually(player.bet, message.multiplier);
                }
            }
        }

        //Обновление карт на столе
        foreach (Player player in message.players)
        {
            if (player.last_move != null && (player.last_move.Length > 0 || player.missed_turns > 0))
            {
                tableManager.UpdateTable(new SetTurnMessage(player.user_id, player.last_move, player.missed_turns > 0));
            }
        }

        //Kitty
        if (message.kitty_cards != null)
        {
            landlordDeck.Clear();
            foreach (int card in message.kitty_cards)
                landlordDeck.Add(cards[card]);
        }
        if (landlordDeck.Count > 0)
                landlordDeck = Card.SortCardsByValue(landlordDeck);

        //Обработка стадии игры
        if (message.landlord.selected)
        {
            gameState = GameState.game;

            mainPlayer.HandleLandlord(message.landlord.user_id);
            player2.HandleLandlord(message.landlord.user_id);
            player3.HandleLandlord(message.landlord.user_id);

            cardOnTable1.AssignView(landlordDeck[0].cardView);
            cardOnTable2.AssignView(landlordDeck[1].cardView);
            cardOnTable3.AssignView(landlordDeck[2].cardView);
        }
        else
        {
            gameState = GameState.chooseLandlord;

            mainPlayer.HideSprite();
            player2.HideSprite();
            player3.HideSprite();
        }

        //Установка таймера
        if (message.current_player_id != null)
        {
            mainPlayer.SetTimer(message.current_player_id);
            player2.SetTimer(message.current_player_id);
            player3.SetTimer(message.current_player_id);
        }

        webGameManager.connectionManager.SendGotGameState();
    }

    private void AssignCardsOnTable(GameStartPayload message)
    {
        landlordDeck.Clear();
        foreach (int card in message.landlord_cards)
            landlordDeck.Add(cards[card]);
        landlordDeck = Card.SortCardsByValue(landlordDeck);
        cardOnTable1.AssignView(landlordDeck[0].cardView);
        cardOnTable2.AssignView(landlordDeck[1].cardView);
        cardOnTable3.AssignView(landlordDeck[2].cardView);

        if (webGameManager.playerId == message.landlord_id || webGameManager.connectionManager.connectionProps.user_id == message.landlord_id)
        {
            StartCoroutine(DrawCards(3, true));
        }

    }

    private void HandleLandlord(long value, int val)
    {
        Debug.Log("LANDLORD MAKED! id = " + value);
        if (!webGameManager.isMultiplayer)
        {
            cardOnTable1.AssignView(landlordDeck[0].cardView);
            cardOnTable2.AssignView(landlordDeck[1].cardView);
            cardOnTable3.AssignView(landlordDeck[2].cardView);
        }


        if (mainPlayer.playerId == value)
            try
            {
                StartCoroutine(DrawCards(3, true)); // Landlords cccards
            }
            catch { }
        if (gameState != GameState.game)
        {
            gameState = GameState.game;
        }

    }

    public static long GetNextPlayer(long prevPlayer)
    {
        long id = 0;
        switch (prevPlayer)
        {
            case 1:
                id = 2;
                break;
            case 2:
                id = 3;
                break;
            case 3:
                id = 1;
                break;
        }
        return id;
    }

    public Card[] GetCardsByIndexes(int[] indexes)
    {
        Card[] returnCards = new Card[indexes.Length];
        for (int i = 0; i < indexes.Length; i++)
        {
            returnCards[i] = cards[indexes[i]];
        }
        return returnCards;
    }

    public List<Card> GetCardsListByIndexes(int[] indexes)
    {
        List<Card> returnCards = new List<Card>();
        for (int i = 0; i < indexes.Length; i++)
        {
            returnCards.Add(cards[indexes[i]]);
        }
        return returnCards;
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }
}

public enum GameState
{
    preGame,
    chooseLandlord,
    game
}