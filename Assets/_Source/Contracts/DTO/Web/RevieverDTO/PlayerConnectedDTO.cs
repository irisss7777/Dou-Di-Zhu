using System;

namespace _Source.Contracts.DTO.Web
{
    [Serializable]
    public struct PlayerConnectedDTO
    {
        public string UserId { get; private set; }
        public string UserName { get; private set; }
        public string LobbyId { get; private set; }
        public int LobbyPlayers { get; private set; }
        public int MaxLobbyPlayers { get; private set; }

        public PlayerConnectedDTO(string userId, string userName, string lobbyId, int lobbyPlayers, int maxLobbyPlayers)
        {
            UserId = userId;
            UserName = userName;
            LobbyId = lobbyId;
            LobbyPlayers = lobbyPlayers;
            MaxLobbyPlayers = maxLobbyPlayers;
        }
    }
}