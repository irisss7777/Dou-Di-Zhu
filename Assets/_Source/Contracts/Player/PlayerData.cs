using _Source.Contracts.GameLobby;

namespace _Source.Contracts.Player
{
    public struct PlayerData
    {
        public string UserId { get; private set; }
        public string Name { get; private set; }
        public IGameLobbyModel GameLobby { get; private set; }
        
        public PlayerData(string userId, string name, IGameLobbyModel gameLobby)
        {
            UserId = userId;
            Name = name;
            GameLobby = gameLobby;
        }
    }
}