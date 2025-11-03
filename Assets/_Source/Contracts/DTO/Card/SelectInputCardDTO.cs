using _Source.Contracts.Card;

namespace _Source.Contracts.DTO.Card
{
    public struct SelectInputCardDTO
    {
        public bool InCollider { get; private set; }
        public CardData CardData { get; private set; }
        public bool Select { get; private set; }

        public SelectInputCardDTO(CardData cardData, bool select, bool inCollider)
        {
            CardData = cardData;
            Select = select;
            InCollider = inCollider;
        }
    }
}