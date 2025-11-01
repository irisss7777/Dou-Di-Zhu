[System.Serializable]
public class SetTurnMessage : Message
{
    public long playerId; // player that ends Turn

    public bool isMainGame; // 0 - landlord selection, 1 - main game
    public int currentBid; // for landlord selection
    public long bidPlayer; // player, who made last bid

    public int[] cardsPlaced; // only for main game, indexes of cards in deck
    
    public bool isPass; // for all game types;


    public SetTurnMessage(long playerId, int currentBid, long bidPlayer, bool isPass) // for landlord selection
    {
        this.type = "set_turn";
        this.playerId = playerId;
        this.isMainGame = false;
        this.currentBid = currentBid;
        this.bidPlayer = bidPlayer;
        this.isPass = isPass;
    }

    public SetTurnMessage(long playerId, int[] cardsPlaced, bool isPass) // for main game
    {
        this.type = "set_turn";
        this.playerId = playerId;
        this.isMainGame = true;
        this.cardsPlaced = cardsPlaced;
        this.isPass = isPass;
    }
}