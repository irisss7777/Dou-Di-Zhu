using UnityEngine;

namespace _Source.Contracts.DTO.Web
{
    public struct CanUseCardReceiverDTO
    {
        public string UserId  { get; private set; }
        public string UserName { get; private set; }
        public string LobbyId { get; private set; }
        public bool Can { get; private set; }
        
        public CanUseCardReceiverDTO(string userName, string lobbyId, string userId, bool can)
        {
            UserName = userName;
            LobbyId = lobbyId;
            UserId = userId;
            Can = can;
        }
    }
}