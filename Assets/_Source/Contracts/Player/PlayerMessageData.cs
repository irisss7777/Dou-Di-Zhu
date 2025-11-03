using _Source.Contracts.DTO.Card;
using _Source.Contracts.DTO.Player;
using _Source.Contracts.DTO.Web;
using MessagePipe;

namespace _Source.Contracts.Player
{
    public struct PlayerMessageData
    {
        public IPublisher<CurrentPlayerAddedDTO> Player { get; private set; }
        public ISubscriber<AddCardDTO> Add { get; private set; }
        public ISubscriber<SelectCardDTO> Select { get; private set; }
        public IPublisher<CardMoveDTO> CardModePublisher { get; private set; }
        public IPublisher<SelectViewCardDTO> SelectView { get; private set; }

        public PlayerMessageData(IPublisher<CurrentPlayerAddedDTO> player, ISubscriber<AddCardDTO> add, ISubscriber<SelectCardDTO> select, IPublisher<CardMoveDTO> cardModePublisher, IPublisher<SelectViewCardDTO> selectViewCardPublisher)
        {
            Player = player;
            Add = add;
            Select = select;
            CardModePublisher = cardModePublisher;
            SelectView = selectViewCardPublisher;
        }
    }
}