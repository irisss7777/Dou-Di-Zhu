using System;

namespace Web.New.Messages.Payloads.ServerClient
{
    [Serializable]
    public class GameOverPayload {
        public long winner_id;
        public GameResult[] results;
    }
}