using _Source.Contracts.Player;

namespace _Source.Contracts.GameLobby
{
    public interface IGameLobbyModel
    {
        public void SetupLobby(int maxPlayerCount);
        public void AddPlayer(IPlayerModel player);

        public void RemovePlayer(string name);
    }
}