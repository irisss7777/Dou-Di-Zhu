using _Source.Contracts.Card;

namespace _Source.Contracts.CardHandler
{
    public interface ICardHandlerModel
    {
        public void InitCardGrid(ICardGridModel cardGridModel);
        public void AddCard(ICardModel card);
        public void SelectCard(CardData cardData, bool select);
        public void RemoveCard(CardData cardData);
        public ICardModel GetCard(CardData cardData);
    }
}