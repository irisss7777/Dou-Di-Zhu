[System.Serializable]
public class PlayerInfoMessage
{
    public int playerId;
    public string nickname;
    public int points;

    public PlayerInfoMessage(int playerId, string nickname, int points)
    {
        this.playerId = playerId;
        this.nickname = nickname;
        this.points = points;
    }
}
