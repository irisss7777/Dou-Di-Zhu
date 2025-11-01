using _Source.Contracts.Card;

namespace _Source.Contracts.DTO.Card
{
    public struct CardSetBombDTO
    {
        public CardData Data { get; private set; }
        public bool Active { get; private set; }

        public CardSetBombDTO(CardData data, bool active)
        {
            Data = data;
            Active = active;
        }
    }
}