using _Source.Contracts.Card;

namespace _Source.Contracts.DTO.Web
{
    public struct AddCardDTO
    {
        public CardData CardData { get; private set; }
        
        public AddCardDTO(CardData cardData)
        {
            CardData = cardData;
        }
    }
}