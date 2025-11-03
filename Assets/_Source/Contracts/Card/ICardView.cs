using _Source.Contracts.DTO.Card;
using MessagePipe;

namespace _Source.Contracts.Card
{
    public interface ICardView
    {
        public void Initialize(CardData cardData, ISubscriber<CardMoveDTO> cardMode, IPublisher<SelectInputCardDTO> selectInput, ISubscriber<SelectViewCardDTO> selectView);
    }
}