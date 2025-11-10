using _Source.Contracts.Card;

namespace _Source.Contracts.DTO.Card
{
    public struct AddCardOtherDTO
    {
        public string UserName { get; private set; }
        public string LobbyId { get; private set; }
        public CardData[] CardData { get; private set; }
        
        public AddCardOtherDTO(CardData[] cardData, string userName, string lobbyId)
        {
            CardData = cardData;
            UserName = userName;
            LobbyId = lobbyId;
        }
    }
}