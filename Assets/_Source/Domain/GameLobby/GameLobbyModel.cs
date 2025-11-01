using System.Collections.Generic;
using System.Linq;
using _Source.Contracts.DTO.GameStage;
using _Source.Contracts.DTO.Lobby;
using _Source.Contracts.GameLobby;
using _Source.Contracts.Player;
using MessagePipe;
using Zenject;

namespace _Source.Domain.GameLobby
{
    public class GameLobbyModel : IGameLobbyModel
    {
        [Inject] private readonly IPublisher<GameActiveStateDTO> _gameActiveStatePublisher;
        [Inject] private readonly IPublisher<AddPlayerToLobbyDTO> _addPlayerToLobbyPublisher;
        private List<IPlayerModel> _players = new();

        private int _maxPlayerCount;

        public void SetupLobby(int maxPlayerCount)
        {
            _maxPlayerCount = maxPlayerCount;
        }
        
        public void AddPlayer(IPlayerModel player)
        {
            _players.Add(player);

            UpdateLobbyInfo();
        }

        public void RemovePlayer(string name)
        {
            foreach (var player in _players)
            {
                if (player.PlayerData.Name == name)
                {
                    _players.Remove(player);
                    break;
                }
            }

            UpdateLobbyInfo();
        }

        private void UpdateLobbyInfo()
        {
            List<string> playerNames = new();
            
            foreach (var item in _players)
                playerNames.Add(item.PlayerData.Name);
            
            _addPlayerToLobbyPublisher.Publish(new AddPlayerToLobbyDTO(playerNames.ToArray(), _players.Count, _maxPlayerCount));
            _gameActiveStatePublisher.Publish(new GameActiveStateDTO(true));
        }
    }
}