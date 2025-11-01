using System.Collections.Generic;
using System.Linq;
using _Source.Contracts.Card;
using _Source.Contracts.CardHandler;

namespace _Source.Domain.Card
{
    public class CardHandlerModel : ICardHandlerModel
    {
        private List<ICardModel> _cardModels = new();

        public void AddCard(ICardModel card)
        {
            _cardModels.Add(card);
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