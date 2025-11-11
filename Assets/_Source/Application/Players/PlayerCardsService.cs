using System;
using System.Collections.Generic;
using System.Linq;
using _Source.Contracts.Card;
using _Source.Contracts.CardHandler;
using _Source.Contracts.DTO.Card;
using _Source.Contracts.DTO.Player;
using _Source.Contracts.DTO.Web;
using _Source.Contracts.DTO.Web.SendDTO;
using _Source.Contracts.Factory;
using _Source.Contracts.Player;
using _Source.Contracts.Web.SocketMessage;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Source.Application.Players
{
    public class PlayerCardsService
    {
        [Inject] private readonly PlayerMessageBusService _playerMessageBus;
        [Inject] private readonly ICardFactory _cardFactory;
        
        public void CreateCard(CardData[] cardsData, string userID, ICardHandlerModel cardHandlerModel, string myUserId)
        {
            if(userID != myUserId)
                return;
            
            var sortedCards = SortCardsRandomlyByValue(cardsData);

            List<ICardModel> cards = new();
            
            for (int i = 0; i < sortedCards.Length; i++)
            {
                var card = _cardFactory.CreateCardModel(sortedCards[i]);
                cards.Add(card);
            }
            
            cardHandlerModel.AddCards(cards.ToArray()).Forget();
        }
        
        public CardData[] SortCardsRandomlyByValue(CardData[] cardsData)
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

        public void SelectCard(SelectCardDTO message, string userId, ICardHandlerModel cardHandlerModel)
        {
            if(message.UserID != userId)
                return;
            
            bool can = cardHandlerModel.SelectCard(message.CardData, message.Select);
            
            if(!can)
                return;
            
            _playerMessageBus.PublishCardSelected(new SelectViewCardDTO(message.CardData, message.Select));

            List<CardData> cardsData = new();

            foreach (var card in cardHandlerModel.SelectedCardModels)
            {
                cardsData.Add(card.CardData);
            }

            _playerMessageBus.PublishCanUseCard(new CanUseCardDTO(new CardsWebMessage(cardsData.ToArray()), WebSocketMessageType.CAN_USE_CARD));
        }

        public void TryUseCard(ICardHandlerModel cardHandlerModel)
        {
            List<CardData> cardsData = new();

            foreach (var card in cardHandlerModel.SelectedCardModels)
            {
                cardsData.Add(card.CardData);
            }

            _playerMessageBus.PublishCardUsed(new UseCardDTO(new CardsWebMessage(cardsData.ToArray()), WebSocketMessageType.USE_CARD));
        }

        public void UseCard(UseCardReceiverDTO message, ICardHandlerModel cardHandlerModel, PlayerData playerData)
        {
            if(!message.Used)
                return;

            CardData[] cardDatas = message.Cards;
            List<ICardModel> cardsToMove = new();

            foreach (var cardData in cardDatas)
            {
                var card = cardHandlerModel.GetCard(cardData);
                if(card != null)
                    cardsToMove.Add(card);
            }

            if (cardsToMove.Count == 0)
            {
                return;
            }
            
            cardHandlerModel.RemoveCard(cardDatas.ToArray());

            List<ICardModel> remainingCards = cardHandlerModel.CardModels;
    
            cardHandlerModel.ReplaceOnGrid(remainingCards.ToArray());
            _playerMessageBus.PublishCardsMoved(new MoveUsedCardsDTO(playerData, cardsToMove.ToArray()));
            cardHandlerModel.ClearSelection();

            List<CardData> emptyCards = new();

            _playerMessageBus.PublishCanUseCard(new CanUseCardDTO(new CardsWebMessage(emptyCards.ToArray()), WebSocketMessageType.CAN_USE_CARD));
            _playerMessageBus.PublishCardCountUpdated(new PlayerSetCardCountDTO(message.CardsCount, message.UserName));
            _playerMessageBus.PublishCardViewAdded(new PlayerAddCardViewDTO(message.UserName));
        }

        public void UseCardOther(UseCardOtherDTO message, string playerName, PlayerData playerData)
        {
            if (message.UserName == playerName)
            {
                var sortedCards = SortCardsRandomlyByValue(message.Cards);

                List<ICardModel> cards = new();
            
                for (int i = 0; i < sortedCards.Length; i++)
                {
                    var card = _cardFactory.CreateCardModel(sortedCards[i]);
                    cards.Add(card);
                }
                
                _playerMessageBus.PublishCardAddedToOther(new AddCardOtherDTO(message.Cards, playerData.Name, message.LobbyId));
                _playerMessageBus.PublishCardsMoved(new MoveUsedCardsDTO(playerData, cards.ToArray()));
                _playerMessageBus.PublishCardCountUpdated(new PlayerSetCardCountDTO(message.CardsCount, message.UserName));
                _playerMessageBus.PublishCardViewAdded(new PlayerAddCardViewDTO(message.UserName));
            }
        }
    }
}