using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Web.New.Messages.Payloads.ServerClient;
using System.Linq;

public class HandManager : MonoBehaviour
{
    [SerializeField] private Transform handTransform;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float handMaxWidth = 5f;
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private Vector3 cardScale = new Vector3(1, 1, 1);
    [SerializeField] private Button placeButton;
    [SerializeField] private Button hintButton;
    [SerializeField] private Button passButton;
    [SerializeField] private TableManager tableManager;
    [SerializeField] private GameManagerMG gameManagerMG;
    [SerializeField] private GameManager gameManager;

    private WebGameManager webGameManager;
    public Action<int> OnHandResize;

    private float handWidth;
    private Card selectedCard = null;
    private List<Card> chosenCards = new List<Card>();
    public List<Card> cardsInHand = new List<Card>();
    public Card[] lastPlacedCards = new Card[0];

    private void Start()
    {
        if (!gameManagerMG)
            gameManagerMG = FindObjectOfType<GameManagerMG>();
        if (!gameManager)
            gameManager = FindObjectOfType<GameManager>();
        webGameManager = FindObjectOfType<WebGameManager>();
        if (webGameManager)
            webGameManager.eventRouter.OnPlayUpdate += (PlayUpdatePayload message) =>
            {
                Debug.Log("HandManager принял play update");
                if (message.user_id == gameManager.mainPlayer.playerId || message.user_id == webGameManager.playerId || message.user_id == webGameManager.connectionManager.connectionProps.user_id)
                {
                    Debug.Log("HandManager начал обновление руки.");
                    PlaceCards(GetCardsInstances(gameManager.GetCardsListByIndexes(message.cards)));
                }
            };
        hintButton.onClick.AddListener(Hint);
        placeButton.onClick.AddListener(ValidateAndPlaceChosenCards);
        passButton.onClick.AddListener(Pass);
    }

    public void Clear()
    {
        while (chosenCards.Count > 0)
        {
            chosenCards.RemoveAt(0);
        }
        while (cardsInHand.Count > 0)
        {
            var card = cardsInHand[0];
            cardsInHand.RemoveAt(0);
            Destroy(card.gameObject);
        }
        selectedCard = null;
    }

    public void CheckTurn()
    {
        Card[] cardArray = new Card[cardsInHand.Count];
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            cardArray[i] = cardsInHand[i];
        }
        Card[] hintedCards = ComboValidator.FindStrongerCombo(cardArray, lastPlacedCards);
        if (hintedCards == null || hintedCards.Length == 0)
        {
            placeButton.gameObject.SetActive(false);
            hintButton.gameObject.SetActive(false);
        }
        else
        {
            placeButton.gameObject.SetActive(true);
            hintButton.gameObject.SetActive(true);
        }
    }

    public void AddCard(Card card)
    {
        if (cardsInHand.Contains(card))
        {
            Debug.Log("Can not draw card. Card already exists in hand");
            return;
        }
        GameObject cardPrefab = card.gameObject;
        GameObject newCard = Instantiate(cardPrefab, spawnPoint.position, Quaternion.identity, handTransform);
        newCard.transform.localScale = Vector3.zero;
        newCard.AddComponent<CardClickHandler>();

        cardsInHand.Add(newCard.GetComponent<Card>());
        OnHandResize?.Invoke(cardsInHand.Count);
        AdjustHandWidth();

        StartCoroutine(MoveCardToHand(newCard));
    }

    private void AdjustHandWidth()
    {
        switch (cardsInHand.Count)
        {
            case 1:
                handWidth = handMaxWidth / 100f;
                break;
            case 18:
                handWidth = handMaxWidth * cardsInHand.Count * 0.05f;
                break;
            case 19:
                handWidth = handMaxWidth * cardsInHand.Count * 0.05f;
                break;
            case 20:
                handWidth = handMaxWidth * cardsInHand.Count * 0.05f;
                break;
            default:
                handWidth = handMaxWidth * cardsInHand.Count * 0.06f;
                break;
        }
    }

    private IEnumerator MoveCardToHand(GameObject card)
    {
        Vector3 targetPosition = GetCardPosition(cardsInHand.IndexOf(card.GetComponent<Card>()));
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = cardScale;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;
            card.transform.position = Vector3.Lerp(spawnPoint.position, targetPosition, t);
            card.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        card.transform.position = targetPosition;
        card.transform.localScale = endScale;
        UpdateCardPositions();
    }

    public void SortHand()
    {

        cardsInHand = Card.SortCardsByValue(cardsInHand);

        UpdateCardPositions();
    }

    private void UpdateCardPositions()
    {
        AdjustHandWidth();
        int cardCount = cardsInHand.Count;
        if (cardCount == 0) return;

        float startX = -handWidth / 2;
        float spacing = cardsInHand.Count > 1 ? (handWidth / (cardsInHand.Count - 1)) : 0;

        for (int i = 0; i < cardCount; i++)
        {
            cardsInHand[i].SetBombSelectffect(false);

            if (cardsInHand[i].IsIconActive) //         BOMB ICON ASSIGNING!
                cardsInHand[i].SetBombIcon(false);
            if (cardCount - i >= 4)
            {
                if (cardsInHand[i].value == cardsInHand[i + 1].value && cardsInHand[i].value == cardsInHand[i + 2].value && cardsInHand[i].value == cardsInHand[i + 3].value)
                {
                    cardsInHand[i].SetBombIcon(true);
                }
            }
            if (cardCount > 1 && cardsInHand[i].suit == Suits.jokers && cardsInHand.Count > i + 1 && cardsInHand[i + 1].suit == Suits.jokers)
            {
                cardsInHand[i].SetBombIcon(true);
            }

            Vector3 targetPosition = handTransform.position + new Vector3(startX + i * spacing, 0, 0);
            if (cardsInHand[i] == selectedCard || chosenCards.Contains(cardsInHand[i]))
            {
                targetPosition += Vector3.up * 0.3f;
            }

            cardsInHand[i].SetViewLayer(i + 1);
            cardsInHand[i].StartMoveCoroutine(this, targetPosition, moveDuration);
        }
        var comboType = ComboValidator.ValidateCombo(chosenCards.ToArray());
        if (comboType == ComboType.Bomb || comboType == ComboType.MultipleBombs)
        {
            foreach (var card in chosenCards)
            {
                card.SetBombSelectffect(true);
            }
        }
    }

    private Vector3 GetCardPosition(int index)
    {
        float startX = -handWidth / 2;
        float spacing = cardsInHand.Count > 1 ? (handWidth / (cardsInHand.Count - 1)) : 0;
        return handTransform.position + new Vector3(startX + index * spacing, 0, 0);
    }

    public void SelectCard(GameObject card)
    {
        if (selectedCard == card)
        {
            selectedCard = null;
        }
        else
        {
            selectedCard = card.GetComponent<Card>();
        }
        UpdateCardPositions();
        ChooseCard();
    }

    public void ChooseCard()
    {
        if (selectedCard != null && !chosenCards.Contains(selectedCard))
        {
            chosenCards.Add(selectedCard);
            selectedCard = null;
        }
        UpdateCardPositions();
    }

    public void ResetChosenCard(GameObject choosenCard)
    {
        chosenCards.Remove(choosenCard.GetComponent<Card>());
        UpdateCardPositions();
    }

    public void ResetAllCards()
    {
        chosenCards.Clear();
        UpdateCardPositions();
    }

    public bool IsChosen(GameObject card)
    {
        if (chosenCards.Contains(card.GetComponent<Card>()))
            return true;
        else
            return false;
    }

    public void Hint()
    {
        Card[] cardArray = new Card[cardsInHand.Count];
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            cardArray[i] = cardsInHand[i];
        }
        Card[] hintedCards;
        if (lastPlacedCards == null || lastPlacedCards.Length == 0)
        {
            hintedCards = ComboValidator.FindRandomCombo(cardArray);
        }
        else
        {
            hintedCards = ComboValidator.FindStrongerCombo(cardArray, lastPlacedCards);
        }
        Card[] hintedCardsNext = new Card[0];
        if (chosenCards.Count > 0)
        {
            hintedCardsNext = ComboValidator.FindStrongerCombo(cardArray, chosenCards.ToArray());
        }
        if (hintedCardsNext != null && hintedCardsNext.Length > 0)
            hintedCards = hintedCardsNext;
        ResetAllCards();
        if (hintedCards == null)
            return;
        foreach (Card card in hintedCards)
        {
            SelectCard(card.gameObject);
        }
    }

    public void ValidateAndPlaceChosenCards()
    {
        if (tableManager == null) return;
        List<Card> placingCards = new List<Card>(chosenCards);
        Card[] cardArray = new Card[placingCards.Count];
        for (int i = 0; i < placingCards.Count; i++)
        {
            cardArray[i] = placingCards[i];
        }

        if (ComboValidator.IsStrongerCombo(cardArray, lastPlacedCards))
            PlaceCardsRequest(placingCards, cardArray);
        else
            ResetAllCards();
    }

    private List<Card> GetCardsInstances(List<Card> placingCardsData)
    {
        HashSet<int> numbersToPlace = new HashSet<int>(
        placingCardsData.Select(card => card.numberInDeck)
        );
        List<Card> placingCards = cardsInHand
        .Where(card => numbersToPlace.Contains(card.numberInDeck))
        .ToList();
        return placingCards;
    }

    private void PlaceCards(List<Card> placingCards)
    {
        foreach (Card card in placingCards)
        {
            cardsInHand.Remove(card);
        }
        ResetAllCards();

        if (cardsInHand.Count == 0)
            gameManagerMG.SendWinGame();

        OnHandResize?.Invoke(cardsInHand.Count);

        placingCards = Card.SortCardsBySubsets(placingCards);

        List<GameObject> placingCardsGO = new List<GameObject>();
        for (int i = 0; i < placingCards.Count; i++)
            placingCardsGO.Add(placingCards[i].gameObject);
        tableManager.ReceiveCards(placingCardsGO);
        UpdateCardPositions();
    }

    private void PlaceCardsRequest(List<Card> placingCards, Card[] cardArray)
    {

        int[] cardIndexesArray = new int[placingCards.Count];

        for (int i = 0; i < cardArray.Length; i++)
        {
            cardIndexesArray[i] = cardArray[i].numberInDeck;
        }

        gameManagerMG.Turn(cardIndexesArray, ComboValidator.ValidateCombo(cardArray));

        if (!webGameManager.isMultiplayer)
        {
            PlaceCards(placingCards);
        }
        //lastPlacedCards = cardArray;
    }

    public void Pass()
    {
        gameManagerMG.Pass();
    }
}
