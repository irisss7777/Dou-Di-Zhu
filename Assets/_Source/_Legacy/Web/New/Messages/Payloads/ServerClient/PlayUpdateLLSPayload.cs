using System;

namespace Web.New.Messages.Payloads.ServerClient
{
    [Serializable]
    public class PlayUpdateLLSPayload
    {
        public long user_id;
        public int current_bid;
        public bool is_pass;
    }
}