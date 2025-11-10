using _Source.Contracts.Web.SocketMessage;

namespace _Source.Contracts.DTO.Web.SendDTO
{
    public struct PlayerPassDTO
    {
        public WebSocketMessageType Type { get; private set; }
        public string Data { get; private set; }
        
        public PlayerPassDTO(string data, WebSocketMessageType type)
        {
            Type = type;
            Data = data;
        }
    }
}