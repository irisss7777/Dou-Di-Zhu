[System.Serializable]
public class EmojiMessage : Message
{
    public int emojiId;
    public long senderId;
    public long recieverId;

    public EmojiMessage(int emojiId, long senderId, long recieverId)
    {
        this.type = "emoji";
        this.emojiId = emojiId;
        this.senderId = senderId;
        this.recieverId = recieverId;
    }
}