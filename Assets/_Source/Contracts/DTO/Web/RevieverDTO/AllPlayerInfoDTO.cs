namespace _Source.Contracts.DTO.Web
{
    public struct AllPlayerInfoDTO
    {
        public string UserId { get; private set; }
        public string[] UserName { get; private set; }
        public string LobbyId { get; private set; }
        public int LobbyPlayers { get; private set; }
        public int MaxLobbyPlayers { get; private set; }
        public int[] SkinNumber { get; private set; }

        public AllPlayerInfoDTO(string userId, string[] userName, string lobbyId, int lobbyPlayers, int maxLobbyPlayers, int[] skinNumber)
        {
            UserId = userId;
            UserName = userName;
            LobbyId = lobbyId;
            LobbyPlayers = lobbyPlayers;
            MaxLobbyPlayers = maxLobbyPlayers;
            SkinNumber = skinNumber;
        }
    }
}