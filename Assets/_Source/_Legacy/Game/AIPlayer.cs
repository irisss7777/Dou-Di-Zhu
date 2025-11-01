using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    [SerializeField] private bool makeTurns = true;
    public PlayerStatsController player;
    public GameManagerLLS gameManagerLLS;
    public GameManagerMG gameManagerMG;
    [SerializeField] private TextMeshPro username;
    private string[] botNames = {"Trainer Bot", "Robo Bot", "Mega Bot", "Just Bot", "Easy Bot", "Enemy Bot"};

    public int[] landlordCards = new int[0];
    public int[] Deck
    {
        get
        {
            return deck;
        }
        set
        {
            deck = value;

            cardDeck = new List<Card>(0);
            foreach(int cardId in deck)
                cardDeck.Add(gameManager.cards[cardId]);
            cardDeck = Card.SortCardsByValue(cardDeck);
            player.OnHandUpdate(cardDeck.Count);
        }
    } 
    private int[] deck;
    private List<Card> cardDeck;
    private int bid;
    
    private WebGameManager webGameManager;
    private GameManager gameManager;

    void Start()
    {
        webGameManager = FindObjectOfType<WebGameManager>();
        if(webGameManager.isMultiplayer)
        {
            this.enabled = false;
            return;
        }
        gameManager = FindObjectOfType<GameManager>();
        if (makeTurns)
        {
            webGameManager.OnSetTurnRecievedLLS += (SetTurnMessage msg) => { StartCoroutine(SetTurnLLS(msg)); };
            webGameManager.OnSetTurnRecievedMG += (SetTurnMessage msg) => { StartCoroutine(SetTurnMG(msg)); };
        }
        webGameManager.OnLandlord += HandleLandlord;
        if(username)
            username.text = botNames[UnityEngine.Random.Range(0,6)];
    }

    void HandleLandlord(long value, int val)
    {
        if(player.playerId == value)
        {
            var fullDeck = new int[20];
            for(int i = 0; i < 17; i++)
            {
                fullDeck[i] = deck[i];
            }
            for(int i = 0; i < 3; i++)
            {
                fullDeck[i+17] = landlordCards[i];
            }
            Deck = fullDeck;
        }
    }

    IEnumerator SetTurnLLS(SetTurnMessage message)
    {
        if(gameManager.gameState == GameState.chooseLandlord)
        {
            if (GameManager.GetNextPlayer(message.playerId) == player.playerId)
            {
                yield return new WaitForSeconds(Random.Range(3.00f, 6.00f));
                MakeTurnLLS(message.currentBid);
            }
        }
    }

    IEnumerator SetTurnMG(SetTurnMessage message)
    {
        if(gameManager.gameState == GameState.game)
        {
            if(GameManager.GetNextPlayer(message.playerId) == player.playerId)
            {  
                yield return new WaitForSeconds(Random.Range(3.00f,6.00f));
                MakeTurnMG();
            }
        }
    }

    private Card[] GetHint()
    {
        Card[] cardArray = new Card[cardDeck.Count];
        for(int i = 0; i < cardDeck.Count; i++)
        {
            cardArray[i] = cardDeck[i];
        }
        Card[] hintedCards;
        if(gameManagerMG.handManager.lastPlacedCards == null || gameManagerMG.handManager.lastPlacedCards.Count() == 0)
        {
            hintedCards = ComboValidator.FindRandomCombo(cardArray);
        }
        else
        {
            hintedCards = ComboValidator.FindStrongerCombo(cardArray, gameManagerMG.handManager.lastPlacedCards);
        }
        return hintedCards;
    }

    private void MakeTurnLLS(int messageBid)
    {
        bid = UnityEngine.Random.Range(0,2);
        if (bid == 1)
            gameManagerLLS.CallForPlayer(player.playerId);
        else
            gameManagerLLS.PassForPlayer(player.playerId);

    }

    private void MakeTurnMG()
    {
        var cards = GetHint();
        if(cards != null)
        {
            int[] cardIndexesArray = new int[cards.Length];
            for(int i = 0; i < cards.Length; i++)
            {
                cardIndexesArray[i] = cards[i].numberInDeck;
            }
            foreach(Card card in cards)
            {
                cardDeck.Remove(card);
            }
            player.OnHandUpdate(cardDeck.Count);
            gameManagerMG.TurnForPlayer(player.playerId,cardIndexesArray);
            CheckWin();
            
        }
        else
        {
            gameManagerMG.PassForPlayer(player.playerId);
        }
    }

    public void MakeTurnForMainPlayer()
    {
        cardDeck = gameManagerMG.handManager.cardsInHand;
        var cards = GetHint();
        if (cards != null)
        {
            gameManagerMG.handManager.ResetAllCards();
            foreach (Card card in cards)
            {
                gameManagerMG.handManager.SelectCard(card.gameObject);
            }
            gameManagerMG.handManager.ValidateAndPlaceChosenCards();  
        }
        else
        {
            gameManagerMG.Pass();
        }
    }

    void CheckWin()
    {
        if(cardDeck.Count <= 0)
        {
            if(player.isLandlord)
                webGameManager.SendMessage(new WinMessage(1));
            if(!player.isLandlord)
                webGameManager.SendMessage(new WinMessage(0));
        }
    }
}
