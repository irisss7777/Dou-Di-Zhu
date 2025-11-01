[System.Serializable]
public class LandlordMessage : Message
{
    public long playerId;
    public int bid;

    public LandlordMessage(long playerId, int bid) // for landlord selection
    {
        this.type = "landlord";
        this.playerId = playerId;
        this.bid = bid;
    }
}