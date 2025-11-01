using System;

namespace Web.New.Messages.Payloads
{
    [Serializable]
    public class GameResult {
        public long user_id;
        public string role;
        public int result;
    }
}