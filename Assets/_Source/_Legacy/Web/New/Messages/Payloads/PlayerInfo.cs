using System;

namespace Web.New.Messages.Payloads
{
    [Serializable]
    public class PlayerInfo {
        public long user_id;
        public string name;
        public string avatar_url;
    }
}