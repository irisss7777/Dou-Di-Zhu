using System;

namespace Web.New.Messages.Payloads.ServerClient
{
    [Serializable]
    public class DealPayload
    {
        public int[] cards;

        public DealPayload(int[] cards)
        {
            this.cards = cards;
        }
    }
}