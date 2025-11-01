using System;
using _Source.Contracts.Web.SocketMessage;
using Newtonsoft.Json;

namespace _Source.Contracts.Web.Envelope
{
    [Serializable]
    public struct WebSocketEnvelopeData<T>
    {
        [JsonProperty("Type")]
        public string Type { get; private set; }
        [JsonProperty("Data")]
        public T Data { get; private set; }
        
        public WebSocketEnvelopeData(T data, WebSocketMessageType type)
        {
            Type = type.ToString();
            Data = data;
        }
    }
}