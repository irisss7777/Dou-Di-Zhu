using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Web.New.Messages.Payloads.ServerClient;
using System.Linq;

public class TableManager : MonoBehaviour
{
    [SerializeField] private Transform tableTransform1;
    [SerializeField] private Transform tableTransform2;
    [SerializeField] private Transform tableTransform3;

    [SerializeField] private Transform spawnPosition2;
    [SerializeField] private Transform spawnPosition3;

    [SerializeField] private GameObject passText2;
    [SerializeField] private GameObject passText3;


    [SerializeField] private float cardSpacing = 1.5f;
    [SerializeField] private float tableMaxWidth = 5f;
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float cardScale = 0.7f;  
    [SerializeField] private int cardsSortingOrder = 100;  
    private float tableWidth;
    [SerializeField] private List<GameObject> cardsOnTable1 = new List<GameObject>();
    [SerializeField] private List<GameObject> cardsOnTable2 = new List<GameObject>();
    [SerializeField] private List<GameObject> cardsOnTable3 = new List<GameObject>();
    [SerializeField] private WebGameManager webGameManager;
    [SerializeField] private GameManager gameManager;


    void Start()
    {
        if(!gameManager)
            gameManager = FindObjectOfType<GameManager>();
        webGameManager = FindObjectOfType<WebGameManager>();
        if (webGameManager)
        {
            webGameManager.OnSetTurnRecievedMG += UpdateTable;
            webGameManager.eventRouter.OnPlayUpdate += HandlePlayUpdate;
            webGameManager.eventRouter.OnPassUpdate += HandlePassUpdate;
        }
    }

    public void Clear()
    {
        ClearAt(cardsOnTable1);
        ClearAt(cardsOnTable2);
        ClearAt(cardsOnTable3);
    }

    private void ClearAt(List<GameObject> table)
    {
        while(table.Count>0)
        {
            var card = table[0];
            table.RemoveAt(0);
            Destroy(card);
        }
    }

    public void ReceiveCards(List<GameObject> newCards)
    {
        ClearAt(cardsOnTable1);
        foreach (GameObject card in newCards)
        {
            card.transform.SetParent(tableTransform1);
            cardsOnTable1.Add(card);
            card.GetComponent<Card>().SetViewLayer(cardsOnTable1.Count);
        }
        UpdateCardPositions(cardsOnTable1, tableTransform1);
    }

    private void AdjustTableWidth()
    {
        switch (cardsOnTable1.Count)
        {
            case 1:
                tableWidth = tableMaxWidth / 100f;
                break;
            case 2:
                tableWidth = tableMaxWidth / 3f;
                break;
            case 3:
                tableWidth = tableMaxWidth / 2f;
                break;
            case 4:
                tableWidth = tableMaxWidth / 1.5f;
                break;
            default:
                tableWidth = tableMaxWidth;
                break;
        }
    }
    
    private void HandlePlayUpdate(PlayUpdatePayload message)
    {
        var newMsg = new SetTurnMessage(message.user_id, message.cards, false);
        gameManager.gameState = GameState.game;
        UpdateTable(newMsg);
    }

    private void HandlePassUpdate(PassUpdatePayload message)
    {
        var newMsg = new SetTurnMessage(message.user_id, new int[0], true);
        gameManager.gameState = GameState.game;
        UpdateTable(newMsg);
    }

    public void UpdateTable(SetTurnMessage message)
    {
        if (gameManager.gameState != GameState.game)
            return;
        if (gameManager.player2.playerId == message.playerId)
        {
            ClearAt(cardsOnTable2);
            if (message.isPass)
                passText2.SetActive(true);
            else
                passText2.SetActive(false);

            List<Card> sortedCards = new List<Card>();
            foreach (int cardNumber in message.cardsPlaced)
            {
                sortedCards.Add(gameManager.cards[cardNumber]);
            }
            sortedCards = Card.SortCardsBySubsets(sortedCards);

            foreach (Card card in sortedCards)
            {
                GameObject newCard = Instantiate(card.gameObject, spawnPosition2.position, Quaternion.identity, tableTransform2);
                newCard.transform.localScale = Vector3.zero;
                cardsOnTable2.Add(newCard);
                newCard.GetComponent<Card>().SetViewLayer(cardsOnTable2.Count);
            }
            UpdateCardPositions(cardsOnTable2, tableTransform2);
        }
        if (gameManager.player3.playerId == message.playerId)
        {
            ClearAt(cardsOnTable3);
            if (message.isPass)
                passText3.SetActive(true);
            else
                passText3.SetActive(false);

            List<Card> sortedCards = new List<Card>();
            foreach (int cardNumber in message.cardsPlaced)
            {
                sortedCards.Add(gameManager.cards[cardNumber]);
            }
            sortedCards = Card.SortCardsBySubsets(sortedCards);

            foreach (Card card in sortedCards)
            {
                GameObject newCard = Instantiate(card.gameObject, spawnPosition3.position, Quaternion.identity, tableTransform3);
                newCard.transform.localScale = Vector3.zero;
                cardsOnTable3.Add(newCard);
                newCard.GetComponent<Card>().SetViewLayer(cardsOnTable3.Count);
            }
            UpdateCardPositions(cardsOnTable3, tableTransform3);
        }
    }

    private void UpdateCardPositions(List<GameObject> cardsList, Transform targetTransform)
    {
        AdjustTableWidth();
        int cardCount = cardsList.Count;
        if (cardCount == 0) return;
        float spacing = cardSpacing;
        float startX = -(cardsList.Count-1)*spacing / 2;
        
        for (int i = 0; i < cardCount; i++)
        {
            Vector3 targetPosition = targetTransform.position + new Vector3(startX + i * spacing, 0, 0);
            StartCoroutine(MoveCardSmoothly(cardsList[i], targetPosition));
        }
    }

    private IEnumerator MoveCardSmoothly(GameObject card, Vector3 targetPosition)
    {
        Vector3 startPosition = card.transform.position;
        Vector3 startScale = card.transform.localScale;
        Vector3 targetScale = new Vector3(cardScale, cardScale, cardScale); 

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;
            card.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            card.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        card.transform.position = targetPosition;
        card.transform.localScale = targetScale;
    }
}
