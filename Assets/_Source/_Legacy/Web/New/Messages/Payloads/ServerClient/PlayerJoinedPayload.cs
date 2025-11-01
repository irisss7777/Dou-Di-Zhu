using System;

namespace Web.New.Messages.Payloads.ServerClient
{
    [Serializable]
    public class PlayerJoinedPayload {
        public long user_id;
        public string name;
        public string avatar_url;
    }
}