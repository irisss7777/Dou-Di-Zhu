using System;

namespace Web.New.Messages.Payloads.ServerClient
{
    [Serializable]
    public class GameStartPayload
    {
        public long landlord_id;
        public int[] landlord_cards;
        public int final_bid;
    }
}