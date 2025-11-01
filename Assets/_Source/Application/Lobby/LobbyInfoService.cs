using System;
using System.Text;
using _Source.Contracts.DTO.Lobby;
using _Source.Presentation.View.LobbyInfoView;
using MessagePipe;
using Zenject;

namespace _Source.Application.Lobby
{
    public class LobbyInfoService : IDisposable, IInitializable
    {
        [Inject]  private readonly ISubscriber<AddPlayerToLobbyDTO> _playerAddedPublisher;

        private LobbyInfoView _lobbyInfoView;
        
        private DisposableBagBuilder _disposable;

        [Inject]
        private void Construct(LobbyInfoView lobbyInfoView)
        {
            _lobbyInfoView = lobbyInfoView;
        }
        
        public void Initialize()
        {
            _disposable = DisposableBag.CreateBuilder();
            
            _playerAddedPublisher.Subscribe((message) => SetupLobbyInfo(message.Name, message.CurrentPlayerCount, message.MaxPlayerCount)).AddTo(_disposable);
        }

        private void SetupLobbyInfo(string[] names, int currentPlayer, int maxPlayer)
        {
            StringBuilder nameBuilder = new();
            
            foreach (var name in names)
                nameBuilder.AppendLine(name);

            var nameText = nameBuilder.ToString();
            
            _lobbyInfoView.PlayerNamesText.text = nameText;
            _lobbyInfoView.PlayerCountText.text = "(" + currentPlayer + "/" + maxPlayer + ")";
        }

        public void Dispose()
        {
            _disposable.Build().Dispose();
        }
    }
}