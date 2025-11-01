using System;

namespace Web.New.Messages.Payloads.ClientServer
{
    [Serializable]
    public class PlayPayload
    {
        public long lobby_id;
        public long user_id;
        public int[] cards;
        public string combo_type;
    }
}