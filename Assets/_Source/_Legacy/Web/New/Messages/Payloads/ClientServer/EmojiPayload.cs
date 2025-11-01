using System;

namespace Web.New.Messages.Payloads.ClientServer
{
    [Serializable]
    public class EmojiPayload {
        public long lobby_id;
        public long from_user_id;
        public long to_user_id;
        public int emoji_id;
    }
}