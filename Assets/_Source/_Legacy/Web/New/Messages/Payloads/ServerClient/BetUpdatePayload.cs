using System;

namespace Web.New.Messages.Payloads.ServerClient
{
    [Serializable]
    public class BetUpdatePayload
    {
        public long user_id;
        public int bet;
        public int multiplier;
        public int balance;
    }
}