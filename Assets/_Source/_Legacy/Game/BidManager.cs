using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Web.New.Messages.Payloads.ServerClient;

public class BidManager : MonoBehaviour
{
    public TextMeshPro bidText;
    public TextMeshPro multiplierText;
    public int startBid = 0;
    public int multiplier = 0;
    private long landlordId = 0;

    [SerializeField] private PlayerStatsController[] players;

    private WebGameManager webGameManager;
    private GameManager gameManager;
    private GameManagerMG gameManagerMG;

    public int totalBid;

    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private TextMeshProUGUI loseText;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        webGameManager = FindObjectOfType<WebGameManager>();
        gameManagerMG = FindObjectOfType<GameManagerMG>();
        if (webGameManager)
        {
            startBid = webGameManager.userStats.roomStartBid;
            bidText.text = startBid.ToString();
            webGameManager.OnGameRestart += () => { UpdateValues(startBid, 0); };

            if (!webGameManager.isMultiplayer)
            {
                webGameManager.OnLandlord += (long id, int bidMultiplier) => { landlordId = id; StartCoroutine(UpdateWithDelay(startBid, bidMultiplier)); };
                gameManagerMG.OnBomb += (int count) => { int mult = multiplier; while (count > 0) { mult *= 2; count--; } UpdateValues(startBid, mult); };
                webGameManager.eventRouter.OnPlayUpdateLLS += (PlayUpdateLLSPayload message) => { UpdateValues(startBid, message.current_bid); };
            }
            else
            {
                webGameManager.eventRouter.OnBetUpdate += (BetUpdatePayload message) => { if (message.user_id == gameManager.mainPlayer.playerId) UpdateValuesManually(message.bet, message.multiplier); };
            }
        }
    }

    public IEnumerator UpdateWithDelay(int bid, int multiplier)
    {
        yield return new WaitForEndOfFrame();
        UpdateValues(startBid, multiplier);
    }

    public void UpdateValues(int startBid, int multiplier)
    {
        this.startBid = startBid;
        this.multiplier = multiplier;
        try
        {
            int reqBid = this.startBid * multiplier;
            totalBid = CalculateBid(reqBid);
            bidText.text = totalBid.ToString();
            multiplierText.text = "(x" + multiplier + ")";
            winText.text = totalBid.ToString();
            loseText.text = totalBid.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError("Ошибка: " + ex.Message);
        }
    }

    public void UpdateValuesManually(int bid, int multiplier)
    {
        this.multiplier = multiplier;
        try
        {
            totalBid = bid;
            bidText.text = totalBid.ToString();
            multiplierText.text = "(x" + multiplier + ")";
            winText.text = totalBid.ToString();
            loseText.text = totalBid.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError("Ошибка: " + ex.Message);
        }
    }

    private int CalculateBid(int requiredBid)
    {
        int peasant1Balance = 0;
        int peasant2Balance = 0;
        int landlordBalance = 0;

        int peasant1Bid = 0;
        int peasant2Bid = 0;
        int landlordBid = 0;

        int playerPos = 0;
        if (players[0].isLandlord || players[1].isLandlord || players[2].isLandlord)
        {
            foreach (PlayerStatsController player in players)
            {
                if (player.isLandlord) // узнаем, кто из них игрок, а также количество очков у всех
                {
                    landlordBalance = player.pointsCount;
                    if (players[0] == player)
                        playerPos = 1;
                }
                else if (peasant1Balance == 0)
                {
                    peasant1Balance = player.pointsCount;
                    if (players[0] == player)
                        playerPos = 2;
                }
                else
                {
                    peasant2Balance = player.pointsCount;
                    if (players[0] == player)
                        playerPos = 3;
                }
            }

            peasant1Bid = Math.Min(Math.Min(peasant1Balance, requiredBid), webGameManager.userStats.roomMaxBid);
            peasant2Bid = Math.Min(Math.Min(peasant2Balance, requiredBid), webGameManager.userStats.roomMaxBid);

            int totalTeamBid = peasant1Bid + peasant2Bid;
            landlordBid = Math.Min(Math.Min(totalTeamBid, landlordBalance), webGameManager.userStats.roomMaxBid);
            if (totalTeamBid != 0)
            {
                peasant1Bid = (int)Math.Floor(((float)peasant1Bid / totalTeamBid * landlordBid));
                peasant2Bid = (int)Math.Floor((float)peasant2Bid / totalTeamBid * landlordBid);
            }
            else
            {
                peasant1Bid = 0;
                peasant2Bid = 0;
            }
            landlordBid = peasant1Bid + peasant2Bid;
        }
        else
        {

            landlordBalance = players[0].pointsCount;
            peasant1Balance = players[1].pointsCount;
            peasant2Balance = players[2].pointsCount;
            playerPos = 1;
            peasant1Bid = Math.Min(peasant1Balance, requiredBid);
            peasant2Bid = Math.Min(peasant2Balance, requiredBid);
            landlordBid = Math.Min(landlordBalance, requiredBid);

        }

        int returnedValue = 0;

        switch (playerPos)
        {
            case 1:
                returnedValue = landlordBid;
                break;
            case 2:
                returnedValue = peasant1Bid;
                break;
            case 3:
                returnedValue = peasant2Bid;
                break;
            default:
                break;
        }
        return returnedValue;
    }
}
