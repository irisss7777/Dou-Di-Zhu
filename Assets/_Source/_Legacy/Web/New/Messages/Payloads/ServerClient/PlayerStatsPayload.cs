using System;

namespace Web.New.Messages.Payloads.ServerClient
{
    [Serializable]
    public class PlayerStatsPayload {
        public long user_id;
        public int cards_count;
    }
}