using System;

namespace Web.New.Messages.Payloads.ClientServer
{
    [Serializable]
    public class PlayLLSPayload {
        public long lobby_id;
        public long user_id;
        public int bid;
    }
}