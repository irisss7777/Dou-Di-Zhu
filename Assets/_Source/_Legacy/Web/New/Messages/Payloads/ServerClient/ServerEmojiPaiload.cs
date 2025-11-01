using System;

namespace Web.New.Messages.Payloads.ServerClient
{
    [Serializable]
    public class ServerEmojiPayload
    {
        public long from_user_id;
        public long to_user_id;
        public int emoji_id;
    }
}