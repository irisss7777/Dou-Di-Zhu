using _Source.Contracts.Web.SocketMessage;

namespace _Source.Contracts.DTO.Web.SendDTO
{
    public struct CanUseCardDTO
    {
        public WebSocketMessageType Type { get; private set; }
        public CardsWebMessage Data { get; private set; }
        
        public CanUseCardDTO(CardsWebMessage data, WebSocketMessageType type)
        {
            Type = type;
            Data = data;
        }
    }
}