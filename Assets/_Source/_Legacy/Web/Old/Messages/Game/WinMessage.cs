public class WinMessage : Message
{
    public int playerId;
    
    public WinMessage(int playerId)
    {
        this.type = "win";
        this.playerId = playerId;
    }
}
