using System;
using _Source.Contracts.Card;
using _Source.Contracts.CardHandler;
using _Source.Contracts.DTO.Card;
using _Source.Contracts.DTO.Player;
using _Source.Contracts.DTO.Web;
using _Source.Contracts.Player;
using _Source.Domain.Card;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;

namespace _Source.Domain.Player
{
    public class PlayerModel : IDisposable, IPlayerModel
    {
        private PlayerData _playerData;

        private ICardHandlerModel _cardHandlerModel;
        private ICardGridModel _cardGridModel;
        
        private IPublisher<CardMoveDTO> _cardModePublisher;
        
        private readonly IPublisher<CurrentPlayerAddedDTO> _playerAddedPublisher;
        private readonly IPublisher<SelectViewCardDTO> _selectViewCardPublisher;

        private readonly ISubscriber<AddCardDTO> _addCardPublisher;
        private readonly ISubscriber<SelectCardDTO> _selectCardPublisher;

        private DisposableBagBuilder _disposable;

        public PlayerData PlayerData => _playerData;

        public PlayerModel(PlayerData playerData, PlayerMessageData playerMessageData, ICardGridModel gridModel)
        {
            _cardGridModel = gridModel;
            
            SetupPlayer(playerData);

            _cardModePublisher = playerMessageData.CardModePublisher;
            
            _disposable = DisposableBag.CreateBuilder();

            _playerAddedPublisher = playerMessageData.Player;
            _selectViewCardPublisher = playerMessageData.SelectView;
            
            _addCardPublisher = playerMessageData.Add;
            _selectCardPublisher = playerMessageData.Select;
            
            _addCardPublisher.Subscribe((message) => CreateCard(message.CardData).Forget()).AddTo(_disposable);
            _selectCardPublisher.Subscribe((message) => SelectCard(message)).AddTo(_disposable);
            
            _playerAddedPublisher.Publish(new CurrentPlayerAddedDTO(_playerData.Name, _playerData.UserId));
        }

        private void SetupPlayer(PlayerData playerData)
        {
            _playerData = playerData;

            _cardHandlerModel = new CardHandlerModel();
            
            _cardHandlerModel.InitCardGrid(_cardGridModel);
            
            _playerData.GameLobby.AddPlayer(this);
        }
        
        private async UniTask CreateCard(CardData[] cardsData)
        {
            for (int i = 0; i < cardsData.Length; i++)
            {
                var card = new CardModel(cardsData[i], _cardModePublisher);

                _cardHandlerModel.AddCard(card);
                await UniTask.Delay(250);
            }
        }

        public void SelectCard(SelectCardDTO message)
        {
            _cardHandlerModel.SelectCard(message.CardData, message.Select);
            
            _selectViewCardPublisher.Publish(new SelectViewCardDTO(message.CardData, message.Select));
        }

        public void Dispose()
        {
            _playerData.GameLobby.RemovePlayer(_playerData.Name);
            
            _disposable.Build().Dispose();
        }
    }
}