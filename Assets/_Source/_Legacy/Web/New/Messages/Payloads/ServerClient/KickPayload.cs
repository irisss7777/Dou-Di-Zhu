using System;

namespace Web.New.Messages.Payloads.ServerClient
{
    [Serializable]
    public class KickPayload {
        public long user_id;
        public string reason; // "ping_timeout" или "not_enough_balance"
    }
}