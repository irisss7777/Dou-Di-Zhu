using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStatsMessage : Message
{
    public long playerId;
    public string nickname;
    public int pointsCount;
    public bool isLandlord;
    public int cardsCount;
    public string[] cardsPlayed;

    public PlayerStatsMessage(long playerId, string nickname, int pointsCount, bool isLandlord, int cardsCount, string[] cardsPlayed)
    {
        this.type = "player_stats";
        this.playerId = playerId;
        this.nickname = nickname;
        this.pointsCount = pointsCount;
        this.isLandlord = isLandlord;
        this.cardsCount = cardsCount;
        this.cardsPlayed = cardsPlayed;
    }
}
