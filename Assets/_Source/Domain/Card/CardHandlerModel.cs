using System.Collections.Generic;
using System.Linq;
using _Source.Contracts.Card;
using _Source.Contracts.CardHandler;
using UnityEngine;

namespace _Source.Domain.Card
{
    public class CardHandlerModel : ICardHandlerModel
    {
        private List<ICardModel> _cardModels = new();
        private ICardGridModel _cardGridModel;

        public void InitCardGrid(ICardGridModel cardGridModel)
        {
            _cardGridModel = cardGridModel;
        }

        public void AddCard(ICardModel card)
        {
            if (_cardGridModel != null)
            {
                Vector2 targetPosition = _cardGridModel.TryPlaceCard(card);
                float duration = _cardGridModel.Duration;
                card.MoveCard(targetPosition, duration);
            }

            _cardModels.Add(card);
        }

        public void SelectCard(CardData cardData, bool select)
        {
            ICardModel card = _cardModels.First(x => x.CardData.Equals(cardData));

            Vector2 targetPosition = _cardGridModel.GetSelectPosition(card, select);
            float duration = _cardGridModel.Duration;
            card.MoveCard(targetPosition, duration);
        }
        
        public void RemoveCard(CardData cardData)
        {
            ICardModel card = _cardModels.First(x => x.CardData.Equals(cardData));
            _cardModels.Remove(card);
            card.Dispose();
        }
        
        public ICardModel GetCard(CardData cardData)
        {
            ICardModel card = _cardModels.First(x => x.CardData.Equals(cardData));
            return card;
        }
    }
}