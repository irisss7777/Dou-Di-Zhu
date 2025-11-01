using System;

namespace Web.New.Messages.Payloads.ClientServer
{
    [Serializable]
    public class UpdateSoftBalancePayload
    {
        public long user_id;
        public int balance;

        public UpdateSoftBalancePayload(long user_id, int balance)
        {
            this.user_id = user_id;
            this.balance = balance;
        }
    }
}