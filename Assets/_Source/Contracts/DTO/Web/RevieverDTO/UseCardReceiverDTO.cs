using _Source.Contracts.Card;
using UnityEngine;

namespace _Source.Contracts.DTO.Web
{
    public struct UseCardReceiverDTO
    {
        public string UserId  { get; private set; }
        public string UserName { get; private set; }
        public string LobbyId { get; private set; }
        public bool Used { get; private set; }
        public CardData[] Cards { get; private set; }
        public int CardsCount { get; private set; }

        
        public UseCardReceiverDTO(string userName, string lobbyId, string userId, bool used, CardData[] cards, int cardsCount)
        {
            UserName = userName;
            LobbyId = lobbyId;
            UserId = userId;
            Used = used;
            Cards = cards;
            CardsCount = cardsCount;
        }
    }
}