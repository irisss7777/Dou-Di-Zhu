using _Source.Contracts.Card;
using _Source.Contracts.DTO.Web;

namespace _Source.Contracts.DTO.Card
{
    public struct DeselectCardDTO
    {
        public CardData CardData { get; private set; }

        public DeselectCardDTO(CardData cardData)
        {
            CardData = cardData;
        }
    }
}