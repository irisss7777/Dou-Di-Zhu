namespace _Source.Contracts.DTO.Web
{
    public struct SidePlayerConnectedDTO
    {
        public string UserId  { get; private set; }
        public string UserName { get; private set; }
        public int LobbyPlayers { get; private set; }
        public int MaxLobbyPlayers { get; private set; }

        public SidePlayerConnectedDTO(string userName, int lobbyPlayers, int maxLobbyPlayers, string userId)
        {
            UserName = userName;
            LobbyPlayers = lobbyPlayers;
            MaxLobbyPlayers = maxLobbyPlayers;
            UserId = userId;
        }
    }
}