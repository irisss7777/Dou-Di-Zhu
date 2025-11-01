using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Web.New.Messages.Payloads.ServerClient;

public class GameManagerLLS : MonoBehaviour
{
    public int currentTurn = 0;
    public long bidPlayer = 0;
    public int passesCount = 0;
    public GameObject landlordChooseButtons;
    public TextMeshProUGUI landlordChooseButtonText;

    [SerializeField] private PlayerStatsController player1;
    [SerializeField] private PlayerStatsController player2;
    [SerializeField] private PlayerStatsController player3;

    [SerializeField] private GameTimer timer;

    private WebGameManager webGameManager;
    [SerializeField] private GameManager gameManager;

    void Start()
    {
        if (!gameManager)
            gameManager = FindObjectOfType<GameManager>();
        webGameManager = FindObjectOfType<WebGameManager>();
        if (webGameManager)
        {
            webGameManager.OnSetTurnRecievedLLS += HandleSetTurnLLS;
            webGameManager.OnLandlord += HandleLandlord;
            webGameManager.OnGameRestart += HandleRestart;

            webGameManager.eventRouter.OnAuctionTurnRequest += HandleTurnRequest;
            //webGameManager.OnPlayerStatsRecieved += HandlePlayerStats;
        }
    }

    void HandleRestart()
    {
        landlordChooseButtons.SetActive(false);
        currentTurn = 0;
        bidPlayer = 0;
        passesCount = 0;

        player1.canBid = true;
        player2.canBid = true;
        player3.canBid = true;
    }

    private void HandleLandlord(long value, int val)
    {
        landlordChooseButtons.SetActive(false);
    }

    private void HandleTurnRequest(AuctionTurnRequestPayload request)
    {
        currentTurn = request.current_bid;
        if (request.current_player == gameManager.mainPlayer.playerId)
        {
            landlordChooseButtons.SetActive(true);
            timer.StartTimer();
            ChangeButtonText();
        }
        else
        {
            landlordChooseButtons.SetActive(false);
            timer.StopTimer();
        }
    }

    private void HandleSetTurnLLS(SetTurnMessage message)
    {
        if (gameManager.gameState == GameState.chooseLandlord)
        {
            currentTurn = message.currentBid + 1;
            bidPlayer = message.bidPlayer;
            landlordChooseButtons.SetActive(false);
            //controlButtons.SetActive(false);
            if (message.isPass == true)
            {
                if (player1.playerId == message.playerId)
                    player1.canBid = false;
                if (player2.playerId == message.playerId)
                    player2.canBid = false;
                if (player3.playerId == message.playerId)
                    player3.canBid = false;
                passesCount++;
            }
            if (GameManager.GetNextPlayer(message.playerId) == gameManager.mainPlayer.playerId)
            {
                if (player1.canBid)
                {
                    landlordChooseButtons.SetActive(true);
                    timer.StartTimer();
                    ChangeButtonText();
                }
                else
                {
                    Pass();
                }
                //if(gameManager.gameState == GameState.game)
                //    controlButtons.SetActive(true);
            }
        }
    }

    private void ChangeButtonText()
    {
        string buttonLabel = "";
        try
        {
            buttonLabel = FindObjectOfType<LanguageManager>().CurrentLanguage["button_lls_call"];
        }
        catch
        {

        }
        switch (currentTurn)
        {
            case 0:
                landlordChooseButtonText.text = buttonLabel + "";
                break;
            case 1:
                landlordChooseButtonText.text = buttonLabel + "";
                break;
            case 2:
                landlordChooseButtonText.text = buttonLabel + "";
                break;
        }
    }

    public void Call()
    {
        timer.StopTimer();
        CallForPlayer(gameManager.mainPlayer.playerId);
    }

    public void CallForPlayer(long playerId)
    {
        Debug.Log("CALL");
        if (webGameManager.isMultiplayer)
            webGameManager.connectionManager.SendPlayLLS(currentTurn);
        else
        {
            if (currentTurn == 3)
            {
                switch (passesCount)
                {
                    case 2:
                        webGameManager.SendMessage(new LandlordMessage(playerId, 1));
                        break;
                    case 1:
                        webGameManager.SendMessage(new SetTurnMessage(playerId, currentTurn, playerId, false));
                        break;
                    case 0:
                        webGameManager.SendMessage(new SetTurnMessage(playerId, currentTurn, playerId, false));
                        break;
                }
            }
            else if (currentTurn == 4)
            {
                int multiplier = passesCount == 0 ? 8 : 4;
                webGameManager.SendMessage(new LandlordMessage(playerId, multiplier));
            }
            else
            {
                webGameManager.SendMessage(new SetTurnMessage(playerId, currentTurn, playerId, false));
            }




            //if (currentTurn + 1 == 3)
            //{
            //    webGameManager.SendMessage(new LandlordMessage(playerId, 3)); //message about landlord
            //                                                                  //StartCoroutine(SetLandlordsTurn());
            //}
            //else
            //    webGameManager.SendMessage(new SetTurnMessage(playerId, currentTurn + 1, playerId, false));
        }
    }

    private IEnumerator SetLandlordsTurn()
    {
        yield return new WaitForSeconds(1);
        //webGameManager.SendMessage(new SetTurnMessage(gameManager.mainPlayer.playerId));  
    }

    public void Pass()
    {
        if (gameManager.gameState == GameState.chooseLandlord)
        {
            timer.StopTimer();
            PassForPlayer(gameManager.mainPlayer.playerId);
        }
    }

    public void PassForPlayer(long playerId)
    {
        Debug.Log("PASS");
        if (webGameManager.isMultiplayer)
            webGameManager.connectionManager.SendPlayLLS(0);
        else
        {
            if (gameManager.gameState == GameState.chooseLandlord)
            {

                if (currentTurn == 3)
                {
                    switch (passesCount)
                    {
                        case 2:
                            webGameManager.SendMessage(new RestartMessage());
                            break;
                        case 1:
                            webGameManager.SendMessage(new LandlordMessage(bidPlayer, 1));
                            break;
                        case 0:
                            webGameManager.SendMessage(new SetTurnMessage(playerId, currentTurn, bidPlayer, true));
                            break;
                    }
                }
                else if (currentTurn == 4)
                {
                    int multiplier = passesCount == 0 ? 4 : 2;
                    webGameManager.SendMessage(new LandlordMessage(bidPlayer, multiplier));
                }
                else
                {
                    webGameManager.SendMessage(new SetTurnMessage(playerId, currentTurn, bidPlayer, true));
                }




                //if (playerId == player1.playerId)
                //    player1.canBid = false;
                //if(playerId == player2.playerId)
                //    player2.canBid = false;
                //if(playerId == player3.playerId)
                //    player3.canBid = false;
                //
                //int activePlayersCount = 0;
                //if(player1.canBid) activePlayersCount++;
                //if(player2.canBid) activePlayersCount++;
                //if(player3.canBid) activePlayersCount++;
                //
                //if(activePlayersCount == 1 && currentTurn > 0)
                //{
                //    webGameManager.SendMessage(new LandlordMessage(bidPlayer, currentTurn)); // Message about landlord
                //}
                //else if(activePlayersCount == 0)
                //{
                //    webGameManager.SendMessage(new RestartMessage()); // Restart game
                //}
                //else
                //{
                //    webGameManager.SendMessage(new SetTurnMessage(playerId, currentTurn, bidPlayer, true));
                //}
            }
        }
    }
}