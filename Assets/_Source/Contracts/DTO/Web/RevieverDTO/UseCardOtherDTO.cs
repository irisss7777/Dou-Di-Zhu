using _Source.Contracts.Card;

namespace _Source.Contracts.DTO.Web
{
    public struct UseCardOtherDTO
    {
        public string UserId  { get; private set; }
        public string UserName { get; private set; }
        public string LobbyId { get; private set; }
        public CardData[] Cards { get; private set; }
        public int CardsCount { get; private set; }
        
        public UseCardOtherDTO(string userName, string lobbyId, string userId, CardData[] cards, int cardsCount)
        {
            UserName = userName;
            LobbyId = lobbyId;
            UserId = userId;
            Cards = cards;
            CardsCount = cardsCount;
        }
    }
}