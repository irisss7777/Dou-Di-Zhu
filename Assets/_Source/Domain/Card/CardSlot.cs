using _Source.Contracts.Card;
using UnityEngine;

namespace _Source.Domain.Card
{
    public class CardSlot
    {
        private Vector2 _cardPosition;
        private ICardModel _cardModel;

        public Vector2 CardPosition => _cardPosition;
        public ICardModel CardModel => _cardModel;

        public CardSlot(Vector2 cardsPosition)
        {
            _cardPosition = cardsPosition;
        }

        public void AddCard(ICardModel cardModel)
        {
            _cardModel = cardModel;
        }

        public bool HasCard()
        {
            if (_cardModel != null)
                return true;
            
            return false;
        }

        public ICardModel GetCard()
        {
            var cardModel = _cardModel;
            _cardModel = null;
            return cardModel;
        }
    }
}