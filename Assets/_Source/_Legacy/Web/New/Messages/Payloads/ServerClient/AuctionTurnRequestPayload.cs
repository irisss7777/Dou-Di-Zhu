using System;

namespace Web.New.Messages.Payloads.ServerClient
{
    [Serializable]
    public class AuctionTurnRequestPayload {
        public long current_player;
        public int current_bid;
    }
}