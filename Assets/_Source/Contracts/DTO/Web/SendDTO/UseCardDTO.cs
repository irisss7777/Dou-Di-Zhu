using _Source.Contracts.Web.SocketMessage;

namespace _Source.Contracts.DTO.Web.SendDTO
{
    public struct UseCardDTO
    {
        public WebSocketMessageType Type { get; private set; }
        public CardsWebMessage Data { get; private set; }
        
        public UseCardDTO(CardsWebMessage data, WebSocketMessageType type)
        {
            Type = type;
            Data = data;
        }
    }
}