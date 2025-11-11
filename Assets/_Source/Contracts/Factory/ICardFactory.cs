using _Source.Contracts.Card;

namespace _Source.Contracts.Factory
{
    public interface ICardFactory
    {
        public ICardModel CreateCardModel(CardData cardData);
    }
}