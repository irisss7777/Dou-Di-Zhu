using _Source.Contracts.Card;

namespace _Source.Contracts.CardHandler
{
    public interface ICardHandlerModel
    {
        public void AddCard(ICardModel card);

        public void RemoveCard(CardData cardData);

        public ICardModel GetCard(CardData cardData);
    }
}