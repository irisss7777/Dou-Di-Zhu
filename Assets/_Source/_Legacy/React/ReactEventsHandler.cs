using System;
using UnityEngine;

public class ReactEventsHandler : MonoBehaviour
{
    public void StartGame(string jsonArgs)
    {
        try
        {
            ReactStartMessage message = JsonUtility.FromJson<ReactStartMessage>(jsonArgs);
            if (message == null)
            {
                Debug.LogError("Failed to parse JSON in StartGameSingle");
                return;
            }

            WebGameManager webGameManager = FindObjectOfType<WebGameManager>();
            if (webGameManager)
            {
                webGameManager.playerId = message.playerId;
                webGameManager.roomId = message.roomId;
                webGameManager.roomName = message.roomName;
                webGameManager.roomCurrencyType = message.currencyType;
                webGameManager.gameMode = message.gameMode;
                webGameManager.userStats.roomStartBid = message.roomStartBid;
                webGameManager.userStats.roomMaxBid = message.roomMaxBid;
                webGameManager.userStats.nickname = message.nickname;
                webGameManager.userStats.points = message.points;
                webGameManager.userStats.soundValue = message.soundValue;
                FindObjectOfType<LanguageManager>().SetLanguageByValue((Languages) Enum.Parse(typeof(Languages), message.language));
                webGameManager.GameStartHandler();
            }
        }
        catch
        {
            Debug.Log(jsonArgs);
        }
    }
    
    public void StartTest()
    {
        try
        {
            WebGameManager webGameManager = FindObjectOfType<WebGameManager>();
            if (webGameManager)
            {
                webGameManager.playerId = 7;
                webGameManager.roomId = 7;
                webGameManager.roomName = "";
                webGameManager.roomCurrencyType = 1;
                webGameManager.userStats.roomStartBid = 50;
                webGameManager.userStats.roomMaxBid = 250;
                webGameManager.userStats.nickname = "Test_name";
                webGameManager.userStats.points = 200;
                webGameManager.userStats.soundValue = 0.5f;
                FindObjectOfType<LanguageManager>().SetLanguageByValue((Languages) Enum.Parse(typeof(Languages), "english"));
                webGameManager.GameStartHandler();
            }
        }
        catch
        {
        }
    }
    

    public void StartSinglePlayer(string jsonArgs)
    {
        try
        {
            ReactStartMessage message = JsonUtility.FromJson<ReactStartMessage>(jsonArgs);
            if (message == null)
            {
                Debug.LogError("Failed to parse JSON in StartGameSingle");
                return;
            }

            WebGameManager webGameManager = FindObjectOfType<WebGameManager>();
            if (webGameManager)
            {
                webGameManager.roomName = message.roomName;
                webGameManager.roomCurrencyType = message.currencyType;
                webGameManager.gameMode = message.gameMode;
                webGameManager.userStats.roomStartBid = message.roomStartBid;
                webGameManager.userStats.roomMaxBid = message.roomMaxBid;
                webGameManager.userStats.nickname = message.nickname;
                webGameManager.userStats.points = message.points;
                webGameManager.userStats.soundValue = message.soundValue;
                FindObjectOfType<LanguageManager>().SetLanguageByValue((Languages)Enum.Parse(typeof(Languages), message.language));
                webGameManager.SetSinglePlayer();
            }
        }
        catch
        {
            Debug.Log(jsonArgs);
        }
    }

    public void StartTutorial(string jsonArgs)
    {
        try
        {
            ReactStartMessage message = JsonUtility.FromJson<ReactStartMessage>(jsonArgs);
            if (message == null)
            {
                Debug.LogError("Failed to parse JSON in StartGameSingle");
                return;
            }

            WebGameManager webGameManager = FindObjectOfType<WebGameManager>();
            if (webGameManager)
            {
                webGameManager.roomName = message.roomName;
                webGameManager.userStats.roomStartBid = message.roomStartBid;
                webGameManager.userStats.roomMaxBid = message.roomMaxBid;
                webGameManager.userStats.nickname = message.nickname;
                webGameManager.userStats.points = message.points;
                webGameManager.userStats.soundValue = message.soundValue;
                FindObjectOfType<LanguageManager>().SetLanguageByValue((Languages) Enum.Parse(typeof(Languages), message.language));
                webGameManager.isTutorial = true;
                webGameManager.SetSinglePlayer();
            }
        }
        catch
        {
            Debug.Log(jsonArgs);
        }
    }
}
