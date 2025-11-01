using System;

namespace Web.New.Messages.Payloads.ServerClient
{
    [Serializable]
    public class ConnectedPayload {
        public long lobby_id;
        public int bet;
        public PlayerInfo[] players;
    }
}