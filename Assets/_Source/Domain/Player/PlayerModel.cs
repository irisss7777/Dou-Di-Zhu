using System;
using System.Collections.Generic;
using System.Linq;
using _Source.Application.Players;
using _Source.Contracts.Card;
using _Source.Contracts.CardHandler;
using _Source.Contracts.DTO.Card;
using _Source.Contracts.DTO.Player;
using _Source.Contracts.DTO.Web;
using _Source.Contracts.DTO.Web.SendDTO;
using _Source.Contracts.Player;
using _Source.Contracts.Web.SocketMessage;
using _Source.Domain.Card;
using Cysharp.Threading.Tasks;
using MessagePipe;

namespace _Source.Domain.Player
{
    public class PlayerModel : IDisposable, IPlayerModel
    {
        private PlayerData _playerData;
        private PlayerCardsService _playerCardsService;

        private ICardHandlerModel _cardHandlerModel;
        private ICardGridModel _cardGridModel;
        private IPlayerMessageBus _playerMessageBus;

        public PlayerData PlayerData => _playerData;

        public PlayerModel(PlayerData playerData, ICardGridModel gridModel, IPlayerMessageBus playerMessageBus, PlayerCardsService playerCardsService)
        {
            _cardGridModel = gridModel;
            _playerMessageBus = playerMessageBus;
            _playerCardsService = playerCardsService;
            
            SetupPlayer(playerData);
            SubscribePlayer();
            
            _playerMessageBus.PublishPlayerAdded(new CurrentPlayerAddedDTO(_playerData.Name, _playerData.UserId));
        }

        private void SetupPlayer(PlayerData playerData)
        {
            _playerData = playerData;

            _cardHandlerModel = new CardHandlerModel();
            
            _cardHandlerModel.InitCardGrid(_cardGridModel);
            
            _playerData.GameLobby.AddPlayer(this);
        }

        private void SubscribePlayer()
        {
            _playerMessageBus.SubscribeAddCard((message) => _playerCardsService.CreateCard(message.CardData, message.UserId, _cardHandlerModel, _playerData.UserId));
            _playerMessageBus.SubscribeSelectCard((message) => _playerCardsService.SelectCard(message, _playerData.UserId, _cardHandlerModel));
            _playerMessageBus.SubscribeUseCardInput((message) => _playerCardsService.TryUseCard(_cardHandlerModel));
            _playerMessageBus.SubscribeUseCardFinally((message) => _playerCardsService.UseCard(message, _cardHandlerModel, _playerData));
            _playerMessageBus.SubscribeUseCardOther((message) => _playerCardsService.UseCardOther(message, _playerData.Name, _playerData));
        }
        
        public void Dispose()
        {
            _playerData.GameLobby.RemovePlayer(_playerData.Name);

            _playerMessageBus.Dispose();
            _cardHandlerModel.Dispose();
        }
    }
}