using System;

namespace Web.New.Messages.Payloads.ClientServer
{
    [Serializable]
    public class PlayerReadyPayload {
        public long lobby_id;
        public long user_id;
    }
}