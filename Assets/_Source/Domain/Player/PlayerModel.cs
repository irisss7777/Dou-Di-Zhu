using System;
using System.Collections.Generic;
using System.Linq;
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
using UnityEngine;

namespace _Source.Domain.Player
{
    public class PlayerModel : IDisposable, IPlayerModel
    {
        private PlayerData _playerData;

        private ICardHandlerModel _cardHandlerModel;
        private ICardGridModel _cardGridModel;
        
        private IPublisher<CardMoveDTO> _cardModePublisher;
        private IPublisher<CardDestroyViewDTO> _cardDestroyPublisher;
        
        private readonly IPublisher<CurrentPlayerAddedDTO> _playerAddedPublisher;
        private readonly IPublisher<SelectViewCardDTO> _selectViewCardPublisher;
        private readonly IPublisher<CanUseCardDTO> _canUseCardPublisher;
        private readonly IPublisher<UseCardDTO> _useCardPublisher;
        private readonly IPublisher<MoveUsedCardsDTO> _moveUsedCardsPublisher;
        private readonly IPublisher<AddCardOtherDTO> _addCardPublisher;
        private readonly IPublisher<PlayerSetCardCountDTO> _playerSetCardCountPublisher;
        private readonly IPublisher<PlayerAddCardViewDTO> _playerAddCardViewPublisher;

        private readonly ISubscriber<AddCardDTO> _addCardSubscriber;
        private readonly ISubscriber<SelectCardDTO> _selectCardSubscriber;
        private readonly ISubscriber<UseCardInputDTO> _useCardSubscriber;
        private readonly ISubscriber<UseCardReceiverDTO> _useCardFinallySubscriber;
        private readonly ISubscriber<UseCardOtherDTO> _useCardOtherSubscriber;

        private DisposableBagBuilder _disposable;

        public PlayerData PlayerData => _playerData;

        public PlayerModel(PlayerData playerData, PlayerMessageData playerMessageData, ICardGridModel gridModel)
        {
            _cardGridModel = gridModel;
            
            SetupPlayer(playerData);

            _cardModePublisher = playerMessageData.CardModePublisher;
            _cardDestroyPublisher = playerMessageData.CardDestroyPublisher;
            
            _disposable = DisposableBag.CreateBuilder();

            _playerAddedPublisher = playerMessageData.Player;
            _selectViewCardPublisher = playerMessageData.SelectView;
            _canUseCardPublisher = playerMessageData.CanUseCard;
            _useCardPublisher = playerMessageData.UseCard;
            _moveUsedCardsPublisher = playerMessageData.MoveUsedCardsPublisher;
            _addCardPublisher = playerMessageData.AddCard;
            _playerSetCardCountPublisher = playerMessageData.PlayerSetCardCount;
            _playerAddCardViewPublisher = playerMessageData.PlayerAddCardView;
            
            _addCardSubscriber = playerMessageData.Add;
            _selectCardSubscriber = playerMessageData.Select;
            _useCardSubscriber = playerMessageData.UseCardInput;
            _useCardFinallySubscriber = playerMessageData.UseCardFinally;
            _useCardOtherSubscriber = playerMessageData.UseCardOther;
            
            _addCardSubscriber.Subscribe((message) => CreateCard(message.CardData, message.UserId)).AddTo(_disposable);
            _selectCardSubscriber.Subscribe((message) => SelectCard(message)).AddTo(_disposable);
            _useCardSubscriber.Subscribe((message) => TryUseCard()).AddTo(_disposable);
            _useCardFinallySubscriber.Subscribe((message) => UseCard(message)).AddTo(_disposable);
            _useCardOtherSubscriber.Subscribe((message) => UseCardOther(message)).AddTo(_disposable);
            
            _playerAddedPublisher.Publish(new CurrentPlayerAddedDTO(_playerData.Name, _playerData.UserId));
        }

        private void SetupPlayer(PlayerData playerData)
        {
            _playerData = playerData;

            _cardHandlerModel = new CardHandlerModel();
            
            _cardHandlerModel.InitCardGrid(_cardGridModel);
            
            _playerData.GameLobby.AddPlayer(this);
        }
        
        private void CreateCard(CardData[] cardsData, string userID)
        {
            if(userID != _playerData.UserId)
                return;
            
            var sortedCards = SortCardsRandomlyByValue(cardsData);

            List<ICardModel> cards = new();
            
            for (int i = 0; i < sortedCards.Length; i++)
            {
                var card = new CardModel(sortedCards[i], _cardModePublisher, _cardDestroyPublisher);
                cards.Add(card);
            }
            
            _cardHandlerModel.AddCards(cards.ToArray()).Forget();
        }
        
        private CardData[] SortCardsRandomlyByValue(CardData[] cardsData)
        {
            var groups = cardsData.GroupBy(card => card.CardValue).ToList();
            
            var random = new System.Random();
            var shuffledGroups = groups.OrderBy(g => random.Next()).ToList();
            
            var result = new List<CardData>();
            foreach (var group in shuffledGroups)
            {
                result.AddRange(group);
            }
    
            return result.ToArray();
        }

        private void SelectCard(SelectCardDTO message)
        {
            if(message.UserID != _playerData.UserId)
                return;
            
            bool can = _cardHandlerModel.SelectCard(message.CardData, message.Select);
            
            if(!can)
                return;
            
            _selectViewCardPublisher.Publish(new SelectViewCardDTO(message.CardData, message.Select));

            List<CardData> cardsData = new();

            foreach (var card in _cardHandlerModel.SelectedCardModels)
            {
                cardsData.Add(card.CardData);
            }
            
            _canUseCardPublisher.Publish(new CanUseCardDTO(new CardsWebMessage(cardsData.ToArray()), WebSocketMessageType.CAN_USE_CARD));
        }

        private void TryUseCard()
        {
            List<CardData> cardsData = new();

            foreach (var card in _cardHandlerModel.SelectedCardModels)
            {
                cardsData.Add(card.CardData);
            }
            
            _useCardPublisher.Publish(new UseCardDTO(new CardsWebMessage(cardsData.ToArray()), WebSocketMessageType.USE_CARD));
        }

        private void UseCard(UseCardReceiverDTO message)
        {
            if(!message.Used)
                return;

            CardData[] cardDatas = message.Cards;
            List<ICardModel> cardsToMove = new();

            foreach (var cardData in cardDatas)
            {
                var card = _cardHandlerModel.GetCard(cardData);
                if(card != null)
                    cardsToMove.Add(card);
            }

            if (cardsToMove.Count == 0)
            {
                return;
            }
            
            _cardHandlerModel.RemoveCard(cardDatas.ToArray());

            List<ICardModel> remainingCards = _cardHandlerModel.CardModels;
    
            _cardHandlerModel.ReplaceOnGrid(remainingCards.ToArray());

            _moveUsedCardsPublisher.Publish(new MoveUsedCardsDTO(_playerData, cardsToMove.ToArray()));

            _cardHandlerModel.ClearSelection();

            List<CardData> emptyCards = new();
            _canUseCardPublisher.Publish(new CanUseCardDTO(new CardsWebMessage(emptyCards.ToArray()), WebSocketMessageType.CAN_USE_CARD));
            
            _playerSetCardCountPublisher.Publish(new PlayerSetCardCountDTO(message.CardsCount, message.UserName));
            
            _playerAddCardViewPublisher.Publish(new PlayerAddCardViewDTO(message.UserName));
        }

        private void UseCardOther(UseCardOtherDTO message)
        {
            if (message.UserName == _playerData.Name)
            {
                var sortedCards = SortCardsRandomlyByValue(message.Cards);

                List<ICardModel> cards = new();
            
                for (int i = 0; i < sortedCards.Length; i++)
                {
                    var card = new CardModel(sortedCards[i], _cardModePublisher, _cardDestroyPublisher);
                    cards.Add(card);
                }
                
                _addCardPublisher.Publish(new AddCardOtherDTO(message.Cards, _playerData.Name, message.LobbyId));
                
                _moveUsedCardsPublisher.Publish(new MoveUsedCardsDTO(_playerData, cards.ToArray()));

                _playerSetCardCountPublisher.Publish(new PlayerSetCardCountDTO(message.CardsCount, message.UserName));

                _playerAddCardViewPublisher.Publish(new PlayerAddCardViewDTO(message.UserName));
            }
        }

        public void Dispose()
        {
            _playerData.GameLobby.RemovePlayer(_playerData.Name);
            
            _disposable.Build().Dispose();
        }
    }
}