using _Source.Contracts.Card;

namespace _Source.Contracts.DTO.Card
{
    public struct SelectViewCardDTO
    {
        public CardData CardData { get; private set; }
        public bool Select { get; private set; }

        public SelectViewCardDTO(CardData cardData, bool select)
        {
            CardData = cardData;
            Select = select;
        }
    }
}