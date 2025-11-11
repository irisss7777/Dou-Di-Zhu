using System;
using System.Collections.Generic;
using System.Linq;
using _Source.Contracts.Card;
using _Source.Contracts.CardHandler;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Source.Domain.Card
{
    public class CardHandlerModel : ICardHandlerModel
    {
        private List<ICardModel> _cardModels = new();
        private ICardGridModel _cardGridModel;
        private List<ICardModel> _usedCardModels = new();
        private List<ICardModel> _selectedCardModels = new();

        public List<ICardModel> SelectedCardModels => _selectedCardModels;
        public List<ICardModel> UsedCardModels => _usedCardModels;
        public List<ICardModel> CardModels => _cardModels;

        public void InitCardGrid(ICardGridModel cardGridModel)
        {
            _cardGridModel = cardGridModel;
        }

        public async UniTask AddCards(ICardModel[] cardsModel)
        {
            foreach (var cardModel in cardsModel)
            {
                _cardModels.Add(cardModel);
            }
            
            float duration = _cardGridModel.Duration;
            List <CardMoveData> cardMoveDatas = new();
            
            if (_cardGridModel != null)
            {
                (_cardGridModel as CardGridModel)?.RearrangeCardsWithGrouping(_cardModels.ToArray());

                foreach (var card in _cardModels)
                {
                    Vector2 targetPosition = _cardGridModel.GetSelectPosition(card, false);
                    cardMoveDatas.Add(new CardMoveData(card, targetPosition));
                }
            }
            
            List <CardMoveData> sortedСardsMoveData = cardMoveDatas.OrderBy(data => data.TargetPosition.x).ToList();

            foreach (var cardMoveData in sortedСardsMoveData)
            {
                cardMoveData.CardModel.MoveCard(cardMoveData.TargetPosition, duration);
                await UniTask.Delay(150);
            }
        }

        public bool SelectCard(CardData cardData, bool select)
        {
            ICardModel card = _cardModels.FirstOrDefault(x => x.CardData.Equals(cardData));
            
            if (card == null)
                return false;

            Vector2 targetPosition = _cardGridModel.GetSelectPosition(card, select);
            float duration = _cardGridModel.Duration;
            card.MoveCard(targetPosition, duration);

            if (select)
                _selectedCardModels.Add(card);
            else
                _selectedCardModels.Remove(card);

            return true;
        }
        
        public void RemoveCard(CardData[] cardDatas)
        {
            foreach (var cardData in cardDatas)
            {
                ICardModel card = _cardModels.FirstOrDefault(x => x.CardData.Equals(cardData));

                if (card != null)
                {
                    _selectedCardModels.Remove(card);
                    _cardModels.Remove(card);
                }
            }

            RedistributeCards();
        }
        
        public void ClearSelection()
        {
            foreach (var card in _selectedCardModels.ToList())
            {
                SelectCard(card.CardData, false);
            }
    
            _selectedCardModels.Clear();
        }

        public void ReplaceOnGrid(ICardModel[] cards)
        {
            _cardModels = cards?.ToList() ?? new List<ICardModel>();
            
            if (_cardGridModel != null)
            {
                (_cardGridModel as CardGridModel)?.RearrangeCardsWithGrouping(cards);

                foreach (var card in _cardModels)
                {
                    Vector2 targetPosition = _cardGridModel.GetSelectPosition(card, false);
                    float duration = _cardGridModel.Duration;
                    card.MoveCard(targetPosition, duration);
                }
            }
        }
        
        public ICardModel GetCard(CardData cardData)
        {
            return _cardModels.FirstOrDefault(x => x.CardData.Equals(cardData));
        }

        private void RedistributeCards()
        {
            if (_cardGridModel != null && _cardModels.Count > 0)
            {
                (_cardGridModel as CardGridModel)?.RearrangeCardsWithGrouping(_cardModels.ToArray());
                
                foreach (var card in _cardModels)
                {
                    Vector2 targetPosition = _cardGridModel.GetSelectPosition(card, false);
                    float duration = _cardGridModel.Duration;
                    card.MoveCard(targetPosition, duration);
                }
            }
        }

        public void Dispose()
        {
            foreach (var card in _cardModels)
            {
                card.Dispose();
                _cardModels.Remove(card);
            }
        }
    }
}