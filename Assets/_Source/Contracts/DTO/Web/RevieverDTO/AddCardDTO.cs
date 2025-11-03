using _Source.Contracts.Card;
using UnityEngine;

namespace _Source.Contracts.DTO.Web
{
    public struct AddCardDTO
    {
        public string UserId { get; private set; }
        public string UserName { get; private set; }
        public string LobbyId { get; private set; }
        public CardData[] CardData { get; private set; }
        
        public AddCardDTO(CardData[] cardData, string userId, string userName, string lobbyId)
        {
            CardData = cardData;
            UserId = userId;
            UserName = userName;
            LobbyId = lobbyId;
        }
    }
}