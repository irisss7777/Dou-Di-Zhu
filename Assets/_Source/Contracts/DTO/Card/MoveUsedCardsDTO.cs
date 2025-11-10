using _Source.Contracts.Card;
using _Source.Contracts.Player;

namespace _Source.Contracts.DTO.Card
{
    public struct MoveUsedCardsDTO
    {
        public PlayerData PlayerData { get; private set; }
        public ICardModel[] Cards { get; private set; }

        public MoveUsedCardsDTO(PlayerData playerData, ICardModel[] cards)
        {
            PlayerData = playerData;
            Cards = cards;
        }
    }
}