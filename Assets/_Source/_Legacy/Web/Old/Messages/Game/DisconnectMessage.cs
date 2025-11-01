public class DisconnectMessage : Message
{
    public int playerId;
    
    public DisconnectMessage(int playerId)
    {
        this.type = "disconnect";
        this.playerId = playerId;
    }
}
