using System;
using _Source.Contracts.Card;
using _Source.Contracts.CardHandler;
using _Source.Contracts.DTO.Card;
using _Source.Contracts.DTO.Player;
using _Source.Contracts.DTO.Web;
using _Source.Contracts.GameLobby;
using _Source.Contracts.Player;
using _Source.Domain.Card;
using _Source.Domain.GameLobby;
using MessagePipe;
using UnityEngine;
using Zenject;

namespace _Source.Domain.Player
{
    public class PlayerModel : IMessageHandler<AddCardDTO>, IMessageHandler<SelectCardDTO>, IMessageHandler<DeselectCardDTO>, IDisposable, IPlayerModel
    {
        private PlayerData _playerData;

        private ICardHandlerModel _cardHandlerModel;
        private ICardHandlerModel _selectedCardHandlerModel;
        
        private readonly IPublisher<CurrentPlayerAddedDTO> _playerAddedPublisher;

        private readonly ISubscriber<AddCardDTO> _addCardPublisher;
        private readonly ISubscriber<SelectCardDTO> _selectCardPublisher;
        private readonly ISubscriber<DeselectCardDTO> _deselectCardPublisher;

        private DisposableBagBuilder _disposable;

        public PlayerData PlayerData => _playerData;

        public PlayerModel(PlayerData playerData, IPublisher<CurrentPlayerAddedDTO> player, ISubscriber<AddCardDTO> add, ISubscriber<SelectCardDTO> select, ISubscriber<DeselectCardDTO> deselect)
        {
            SetupPlayer(playerData);
            
            _disposable = DisposableBag.CreateBuilder();

            _playerAddedPublisher = player;
            _addCardPublisher = add;
            _selectCardPublisher = select;
            _deselectCardPublisher = deselect;
            
            _addCardPublisher.Subscribe(this).AddTo(_disposable);
            _selectCardPublisher.Subscribe(this).AddTo(_disposable);
            _deselectCardPublisher.Subscribe(this).AddTo(_disposable);
            
            _playerAddedPublisher.Publish(new CurrentPlayerAddedDTO(_playerData.Name, _playerData.UserId));
        }

        private void SetupPlayer(PlayerData playerData)
        {
            _playerData = playerData;

            _cardHandlerModel = new CardHandlerModel();
            
            _playerData.GameLobby.AddPlayer(this);
        }

        public void Handle(AddCardDTO message)
        {
            var card = new CardModel(message.CardData);
            
            _cardHandlerModel.AddCard(card);
        }


        public void Handle(SelectCardDTO message)
        {
            ICardModel card = _cardHandlerModel.GetCard(message.CardData);
            
            _cardHandlerModel.RemoveCard(card.CardData);
            
            _selectedCardHandlerModel.AddCard(card);
        }
        

        public void Handle(DeselectCardDTO message)
        {
            ICardModel card = _selectedCardHandlerModel.GetCard(message.CardData);
            _selectedCardHandlerModel.RemoveCard(card.CardData);
            _cardHandlerModel.AddCard(card);
        }

        public void Dispose()
        {
            _playerData.GameLobby.RemovePlayer(_playerData.Name);
            
            _disposable.Build().Dispose();
        }
    }
}