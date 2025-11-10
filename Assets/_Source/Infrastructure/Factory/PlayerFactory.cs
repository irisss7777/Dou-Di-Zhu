using System;
using System.Collections.Generic;
using _Source.Contracts.View;
using _Source.Contracts.Card;
using _Source.Contracts.DataBase;
using _Source.Contracts.DTO.Card;
using _Source.Contracts.DTO.Player;
using _Source.Contracts.DTO.Web;
using _Source.Contracts.DTO.Web.SendDTO;
using _Source.Contracts.GameLobby;
using _Source.Contracts.Player;
using _Source.Domain.Card;
using _Source.Domain.GameLobby;
using _Source.Domain.Player;
using _Source.Presentation.StartPositions;
using _Source.Presentation.View.PlayerView;
using MessagePipe;
using UnityEngine;
using Zenject;

namespace _Source.Infrastructure.Factory
{
    public class PlayerFactory : IDisposable, IInitializable
    {
        //Model
        [Inject] private readonly ISubscriber<PlayerConnectedDTO> _playerConnectedSubscriber;
        [Inject] private readonly ISubscriber<SidePlayerConnectedDTO> _sidePlayerConnectedSubscriber;
        [Inject] private readonly ISubscriber<PlayerLeaveDTO> _playerLeaveSubscriber;
        [Inject] private readonly ISubscriber<AllPlayerInfoDTO> _allPlayerInfoSubscriber;

        [Inject] private readonly IPublisher<CurrentPlayerAddedDTO> _playerAddedPublisher;
        [Inject] private readonly IPublisher<SelectViewCardDTO> _selectViewCardPublisher;
        [Inject] private readonly IPublisher<CanUseCardDTO> _canUseCardPublisher;
        [Inject] private readonly IPublisher<UseCardDTO> _useCardPublisher;
        [Inject] private readonly IPublisher<MoveUsedCardsDTO> _moveUsedCardsPublisher;
        [Inject] private readonly IPublisher<AddCardOtherDTO> _addCardPublisher;
        [Inject] private readonly IPublisher<PlayerSetCardCountDTO> _playerSetCardCountPublisher;
        [Inject] private readonly IPublisher<PlayerAddCardViewDTO> _playerAddCardViewPublisher;

        [Inject] private readonly ISubscriber<AddCardDTO> _addCardSubscriber;
        [Inject] private readonly ISubscriber<SelectCardDTO> _selectCardSubscriber;
        [Inject] private readonly ISubscriber<DeselectCardDTO> _deselectCardSubscriber;
        [Inject] private readonly ISubscriber<UseCardInputDTO> _useCardSubscriber;
        [Inject] private readonly ISubscriber<UseCardReceiverDTO> _useCardFinallySubscriber;
        [Inject] private readonly ISubscriber<UseCardOtherDTO> _useCardOtherSubscriber;

        //View
        [Inject] private readonly ISubscriber<PlayerSetCardCountDTO> _playerSetCardCountSubscriber;
        [Inject] private readonly ISubscriber<PlayerPassedDTO> _playerPassedSubscriber;
        [Inject] private readonly ISubscriber<PlayerAddCardViewDTO> _playerAddCardViewSubscriber;
        
        [Inject] private readonly IPublisher<CardMoveDTO> _cardModePublisher;
        [Inject] private IPublisher<CardDestroyViewDTO> _cardDestroyPublisher;

        private ICardGridModel _cardGridModel;
        
        private IGameLobbyModel _gameLobbyModel;
        private IPlayerDataBase _playerDataBase;
        private PlayerStartPositions _startPositions;

        private List<IPlayerView> _playersView = new();

        private PlayerMessageData _playerMessageData;
        
        private DisposableBagBuilder _disposable;
        
        [Inject]
        private void Construct(GameLobbyModel gameLobbyModel, CardGridModel cardGridModel, IPlayerDataBase playerDataBase, PlayerStartPositions startPositions)
        {
            _gameLobbyModel = gameLobbyModel;
            _cardGridModel = cardGridModel;
            _playerDataBase = playerDataBase;
            _startPositions = startPositions;
        }

        public void Initialize()
        {
            _disposable = DisposableBag.CreateBuilder();
            
            _playerConnectedSubscriber.Subscribe((message) => 
                CreatePlayer(message.UserId, message.UserName, message.MaxLobbyPlayers, message.SkinNumber)).AddTo(_disposable);
            
            _sidePlayerConnectedSubscriber.Subscribe((message) => 
                CreateSidePlayer(message.UserId, message.UserName, message.MaxLobbyPlayers, message.SkinNumber)).AddTo(_disposable);
            
            _playerLeaveSubscriber.Subscribe((message) => LeavePlayer(message.UserId, message.UserName, message.MaxLobbyPlayers)).AddTo(_disposable);
            
            _allPlayerInfoSubscriber.Subscribe((message) => 
                DownloadDataAboutAll(message.UserId, message.UserName, message.MaxLobbyPlayers, message.SkinNumber)).AddTo(_disposable);

            _playerMessageData = new PlayerMessageData(_playerAddedPublisher, _addCardSubscriber, _selectCardSubscriber,
                _cardModePublisher, _selectViewCardPublisher, _canUseCardPublisher, _useCardPublisher, _useCardSubscriber, 
                _useCardFinallySubscriber, _cardDestroyPublisher, _moveUsedCardsPublisher, _useCardOtherSubscriber,
                _addCardPublisher, _playerSetCardCountPublisher, _playerAddCardViewPublisher);
        }

        private void CreatePlayer(string userId, string name, int maxPlayerCount, int skinNumber)
        {
            _gameLobbyModel.SetupLobby(maxPlayerCount);
            var player = new PlayerModel(new PlayerData(userId, name, _gameLobbyModel), _playerMessageData, _cardGridModel);
            
            CreatePlayerView(name, skinNumber, true).SetIsMe();
        }

        private void CreateSidePlayer(string userId, string name, int maxPlayerCount, int skinNumber)
        {
            _gameLobbyModel.SetupLobby(maxPlayerCount);
            var player = new PlayerModel(new PlayerData(userId, name, _gameLobbyModel), _playerMessageData, _cardGridModel);

            CreatePlayerView(name, skinNumber);
        }

        private PlayerView CreatePlayerView(string name, int skinNumber, bool isMine = false)
        {
            var playerPrefab = _playerDataBase.PlayerViewPrefab as PlayerView;
            
            PlayerViewPositionData spawnTransform;
            
            if (isMine)
                spawnTransform = _startPositions.GetMyPosition();
            else
                spawnTransform = _startPositions.GetPosition();
            
            _startPositions.SetDataName(name, spawnTransform.Transform);
            
            var playerView = GameObject.Instantiate(playerPrefab, spawnTransform.Transform.position, Quaternion.identity);

            Sprite[] skin = _playerDataBase.GetSkin(skinNumber);
            
            playerView.SetupCharacterSprite(skin, spawnTransform.Direction, spawnTransform.PassText);
            
            playerView.SetupTextInfo(name, spawnTransform.InfoPanelPosition);
            
            playerView.Initialize(_playerSetCardCountSubscriber, _playerPassedSubscriber, _playerAddCardViewSubscriber);

            _playersView.Add(playerView);
            
            return playerView;
        }

        private void LeavePlayer(string userId, string name, int maxPlayerCount)
        {
            _gameLobbyModel.RemovePlayer(name);

            foreach (var playerView in _playersView)
            {
                playerView.Leave(name);
            }
        }

        private void DownloadDataAboutAll(string userId, string[] names, int maxPlayerCount, int[] skinNumber)
        {
            _gameLobbyModel.SetupLobby(maxPlayerCount);
            for(int i = 0; i < names.Length; i++)
            {
                var player = new PlayerModel(new PlayerData(userId, names[i], _gameLobbyModel), _playerMessageData, _cardGridModel);
                CreatePlayerView(names[i], skinNumber[i]);
            }
        }

        public void Dispose()
        {
            _disposable.Build().Dispose();
        }
    }
}