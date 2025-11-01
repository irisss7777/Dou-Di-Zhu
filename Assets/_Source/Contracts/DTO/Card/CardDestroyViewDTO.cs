using _Source.Contracts.Card;

namespace _Source.Contracts.DTO.Card
{
    public struct CardDestroyViewDTO
    {
        public CardData Data { get; private set; }

        public CardDestroyViewDTO(CardData data)
        {
            Data = data;
        }
    }
}