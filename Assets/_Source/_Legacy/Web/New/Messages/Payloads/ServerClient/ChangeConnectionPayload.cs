using System;

namespace Web.New.Messages.Payloads.ServerClient
{
    [Serializable]
    public class ChangeConnectionPayload
    {
        public long user_id;
        public bool is_connected;

        public ChangeConnectionPayload(long user_id, bool is_connected)
        {
            this.user_id = user_id;
            this.is_connected = is_connected;
        }
    }
}