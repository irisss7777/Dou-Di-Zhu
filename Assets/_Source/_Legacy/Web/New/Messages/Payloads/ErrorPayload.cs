using System;

namespace Web.New.Messages.Payloads
{
    [Serializable]
    public class ErrorPayload {
        public int code;
        public string message;
    }
}