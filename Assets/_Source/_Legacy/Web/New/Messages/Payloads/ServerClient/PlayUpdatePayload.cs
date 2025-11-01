using System;

namespace Web.New.Messages.Payloads.ServerClient
{
    [Serializable]
    public class PlayUpdatePayload {
        public long user_id;
        public int[] cards;
    }
}