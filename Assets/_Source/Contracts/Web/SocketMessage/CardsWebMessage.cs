using _Source.Contracts.Card;

namespace _Source.Contracts.Web.SocketMessage
{
    public class CardsWebMessage
    {
        public CardData[] Cards { get; private set; }
        
        public CardsWebMessage(CardData[] cards)
        {
            Cards = cards;
        }
    }
}