using _Source.Contracts.Card;
using _Source.Contracts.DTO.Web;

namespace _Source.Contracts.DTO.Card
{
    public struct SelectCardDTO
    {
        public CardData CardData { get; private set; }

        public SelectCardDTO(CardData cardData)
        {
            CardData = cardData;
        }
    }
}