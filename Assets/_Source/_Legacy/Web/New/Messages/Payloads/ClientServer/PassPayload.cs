using System;

namespace Web.New.Messages.Payloads.ClientServer
{
    [Serializable]
    public class PassPayload {
        public long lobby_id;
        public long user_id;
    }
}