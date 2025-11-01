using System;

namespace Web.New.Connection.Envelope
{
    [Serializable]
    public class WebSocketEnvelope<T> {
        public string @event;
        public T data;
    }
}