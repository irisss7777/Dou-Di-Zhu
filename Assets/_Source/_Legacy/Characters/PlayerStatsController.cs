using System.Collections;
using System.Collections.Generic;
using _Source.Presentation.View.PlayerView;
using TMPro;
using UnityEngine;
using Web.New.Messages.Payloads.ServerClient;

public class PlayerStatsController : MonoBehaviour
{
    public int positionInGame;
    public long playerId;
    public int pointsCount;
    public int cardsCount;
    public bool isLandlord;
    public bool canBid = true;
    private bool isMainPlayer;
    [SerializeField] private TextMeshPro nicknameText;
    [SerializeField] private TextMeshPro[] pointsTexts;
    [SerializeField] private TextMeshPro cardsText;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpritesContainer spritesContainer;
    [SerializeField] private CharacterAnimation animationController;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private GameTimer timer;
    [SerializeField] private GameObject passIndicator;
    [SerializeField] private GameObject connectionLostIndicator;
    private WebGameManager webGameManager;
    private GameManager gameManager;

    void Start()
    {
        webGameManager = FindObjectOfType<WebGameManager>();
        gameManager = FindObjectOfType<GameManager>();
        if(webGameManager)
        {
            webGameManager.OnPlayerStatsRecieved += HandlePlayerStats;
            webGameManager.OnLandlord += HandleLandlord;
            webGameManager.OnSetTurnRecievedLLS += ( SetTurnMessage message ) => { SetTimer(GameManager.GetNextPlayer(message.playerId)); };
            webGameManager.OnSetTurnRecievedMG += ( SetTurnMessage message ) => { SetTimer(GameManager.GetNextPlayer(message.playerId)); };
            
            webGameManager.eventRouter.OnAuctionTurnRequest += (AuctionTurnRequestPayload message) => { SetTimer(message.current_player); };
            webGameManager.eventRouter.OnPlayRequest += (PlayRequestPayload message) => { SetTimer(message.current_player); };
            webGameManager.eventRouter.OnPlayerConnectionChange += ChangeConnectionHandler;

            nicknameText.text = webGameManager.userStats.nickname;
            pointsCount = webGameManager.userStats.points;
            webGameManager.eventRouter.OnPlayerStats += (PlayerStatsPayload message) => { if (playerId == message.user_id) {  cardsCount = message.cards_count; cardsText.text = cardsCount.ToString(); }  };
            if (positionInGame == 1)
            {
                isMainPlayer = true;
                playerId = webGameManager.playerId;
                HandManager handManager = FindObjectOfType<HandManager>();
                handManager.OnHandResize += OnHandUpdate;
            }
            if(positionInGame == 2)
            {
                isMainPlayer = false;
                playerId = webGameManager.playerId == 3 ? 1 : webGameManager.playerId + 1;
            }
            if(positionInGame == 3)
            {
                isMainPlayer = false;
                playerId = webGameManager.playerId == 1 ? 3 : webGameManager.playerId - 1;
            }
        }
    }

    public void HideSprite()
    {
        spriteRenderer.enabled = false;
    }

    public void HandleLandlord(long landlordId, int val = 0)
    {
        Debug.Log("PlayerStats: Делаем видимым игрока: " + playerId);
        if (playerId == landlordId)
        {
            animationController.Sprites = spritesContainer.spritesPig;
            isLandlord = true;
        }
        else
        {

            var list = new List<long>();
            list.Add(gameManager.mainPlayer.playerId);
            list.Add(gameManager.player2.playerId);
            list.Add(gameManager.player3.playerId);
            list.Remove(landlordId);
            list.Sort();
            if (list[0] == playerId)
            {
                animationController.Sprites = spritesContainer.spritesFrog1;
            }
            else
            {
                animationController.Sprites = spritesContainer.spritesFrog2;
            }
        }
        particle.Play();
        spriteRenderer.enabled = true;
    }

    public void ChangeConnectionHandler(ChangeConnectionPayload message)
    {
        if (message.user_id == playerId && connectionLostIndicator)
        {
            connectionLostIndicator.SetActive(!message.is_connected);
        }
    }
    
    public void SetPlayerStats(Player stats)
    {
        if (playerId == stats.user_id)
        {
            nicknameText.text = stats.name;

            cardsCount = stats.cards != null ? stats.cards.Length : 0;
            cardsText.text = cardsCount.ToString();

            if (passIndicator)
                passIndicator.SetActive(stats.missed_turns > 0);

            pointsCount = stats.balance;
            foreach (TextMeshPro pointsText in pointsTexts)
                pointsText.text = pointsCount.ToString();
            
            ChangeConnectionHandler(new ChangeConnectionPayload(playerId, stats.is_connected));
            

            //pointsCount = message.pointsCount;

            //isLandlord = message.isLandlord;


            cardsText.text = cardsCount.ToString();
        }
    }

    private void HandlePlayerStats(PlayerStatsMessage message)
    {
        if (playerId == message.playerId)
        {
            if (isMainPlayer && message.isLandlord && !isLandlord)
            {

            }
            //pointsCount = message.pointsCount;
            cardsCount = message.cardsCount;
            //isLandlord = message.isLandlord;
            nicknameText.text = message.nickname;
            pointsCount = message.pointsCount;
            foreach (TextMeshPro pointsText in pointsTexts)
                pointsText.text = pointsCount.ToString();
            cardsText.text = cardsCount.ToString();
        }
    }

    public void SetTimer(long id)
    {
        if (timer)
        {
            if (playerId == id)
            {
                timer.gameObject.SetActive(true);
                timer.StartTimer();
            }
            else
            {
                try
                {
                    timer.StopTimer();
                    timer.gameObject.SetActive(false);
                }
                catch
                {

                }
            }
        }
    }

    public void OnHandUpdate(int count)
    {
        cardsCount = count;
        SendStats();
    }

    private void SendStats()
    {
        webGameManager.SendMessage(new PlayerStatsMessage(playerId, nicknameText.text, pointsCount, isLandlord,cardsCount,new string[0]));
    }
}
