namespace _Source.Contracts.DTO.Web
{
    public struct PlayerPassedDTO
    {
        public string UserId { get; private set; }

        public string UserName { get; private set; }

        public string LobbyId { get; private set; }

        public PlayerPassedDTO(string userId, string userName, string lobbyId)
        {
            UserId = userId;
            UserName = userName;
            LobbyId = lobbyId;
        }
    }
}