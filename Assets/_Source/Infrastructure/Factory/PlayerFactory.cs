using System;
using _Source.Contracts.Card;
using _Source.Contracts.DTO.Card;
using _Source.Contracts.DTO.Player;
using _Source.Contracts.DTO.Web;
using _Source.Contracts.GameLobby;
using _Source.Contracts.Player;
using _Source.Domain.Card;
using _Source.Domain.GameLobby;
using _Source.Domain.Player;
using MessagePipe;
using UnityEngine;
using Zenject;

namespace _Source.Infrastructure.Factory
{
    public class PlayerFactory : IDisposable, IInitializable
    {
        [Inject] private readonly ISubscriber<PlayerConnectedDTO> _playerConnectedSubscriber;
        [Inject] private readonly ISubscriber<SidePlayerConnectedDTO> _sidePlayerConnectedSubscriber;
        [Inject] private readonly ISubscriber<PlayerLeaveDTO> _playerLeaveSubscriber;
        [Inject] private readonly ISubscriber<AllPlayerInfoDTO> _allPlayerInfoSubscriber;

        [Inject] private readonly IPublisher<CurrentPlayerAddedDTO> _playerAddedPublisher;
        [Inject] private readonly IPublisher<SelectViewCardDTO> _selectViewCardPublisher;
        [Inject] private readonly IPublisher<CardMoveDTO> _cardModePublisher;
        [Inject] private readonly ISubscriber<AddCardDTO> _addCardPublisher;
        [Inject] private readonly ISubscriber<SelectCardDTO> _selectCardPublisher;
        [Inject] private readonly ISubscriber<DeselectCardDTO> _deselectCardPublisher;
        private ICardGridModel _cardGridModel;
        
        private IGameLobbyModel _gameLobbyModel;

        private PlayerMessageData _playerMessageData;
        
        private DisposableBagBuilder _disposable;
        
        [Inject]
        private void Construct(GameLobbyModel gameLobbyModel, CardGridModel cardGridModel)
        {
            _gameLobbyModel = gameLobbyModel;
            _cardGridModel = cardGridModel;
        }

        public void Initialize()
        {
            _disposable = DisposableBag.CreateBuilder();
            
            _playerConnectedSubscriber.Subscribe((message) => CreatePlayer(message.UserId, message.UserName, message.MaxLobbyPlayers)).AddTo(_disposable);
            _sidePlayerConnectedSubscriber.Subscribe((message) => CreateSidePlayer(message.UserId, message.UserName, message.MaxLobbyPlayers)).AddTo(_disposable);
            _playerLeaveSubscriber.Subscribe((message) => LeavePlayer(message.UserId, message.UserName, message.MaxLobbyPlayers)).AddTo(_disposable);
            _allPlayerInfoSubscriber.Subscribe((message) => DownloadDataAboutAll(message.UserId, message.UserName, message.MaxLobbyPlayers)).AddTo(_disposable);

            _playerMessageData = new PlayerMessageData(_playerAddedPublisher, _addCardPublisher, _selectCardPublisher,
                _cardModePublisher, _selectViewCardPublisher);
        }

        private void CreatePlayer(string userId, string name, int maxPlayerCount)
        {
            _gameLobbyModel.SetupLobby(maxPlayerCount);
            var player = new PlayerModel(new PlayerData(userId, name, _gameLobbyModel), _playerMessageData, _cardGridModel);
        }

        private void CreateSidePlayer(string userId, string name, int maxPlayerCount)
        {
            _gameLobbyModel.SetupLobby(maxPlayerCount);
            var player = new PlayerModel(new PlayerData(userId, name, _gameLobbyModel), _playerMessageData, _cardGridModel);
        }

        private void LeavePlayer(string userId, string name, int maxPlayerCount)
        {
            _gameLobbyModel.RemovePlayer(name);
        }

        private void DownloadDataAboutAll(string userId, string[] names, int maxPlayerCount)
        {
            _gameLobbyModel.SetupLobby(maxPlayerCount);
            foreach (var name in names)
            {
                var player = new PlayerModel(new PlayerData(userId, name, _gameLobbyModel), _playerMessageData, _cardGridModel);
            }
        }

        public void Dispose()
        {
            _disposable.Build().Dispose();
        }
    }
}