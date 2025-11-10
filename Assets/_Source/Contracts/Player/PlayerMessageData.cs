using _Source.Contracts.DTO.Card;
using _Source.Contracts.DTO.Player;
using _Source.Contracts.DTO.Web;
using _Source.Contracts.DTO.Web.SendDTO;
using MessagePipe;

namespace _Source.Contracts.Player
{
    public struct PlayerMessageData
    {
        public IPublisher<CurrentPlayerAddedDTO> Player { get; private set; }
        public ISubscriber<AddCardDTO> Add { get; private set; }
        public ISubscriber<SelectCardDTO> Select { get; private set; }
        public ISubscriber<UseCardInputDTO> UseCardInput { get; private set; }
        public ISubscriber<UseCardReceiverDTO> UseCardFinally { get; private set; }
        public ISubscriber<UseCardOtherDTO> UseCardOther { get; private set; }
        
        public IPublisher<CardMoveDTO> CardModePublisher { get; private set; }
        public IPublisher<CardDestroyViewDTO> CardDestroyPublisher { get; private set; }
        public IPublisher<MoveUsedCardsDTO> MoveUsedCardsPublisher { get; private set; }
        public IPublisher<PlayerSetCardCountDTO> PlayerSetCardCount { get; private set; }
        public IPublisher<PlayerAddCardViewDTO> PlayerAddCardView { get; private set; }
        
        public IPublisher<SelectViewCardDTO> SelectView { get; private set; }
        public IPublisher<CanUseCardDTO> CanUseCard { get; private set; }
        public IPublisher<UseCardDTO> UseCard { get; private set; }
        public IPublisher<AddCardOtherDTO> AddCard { get; private set; }

        public PlayerMessageData(IPublisher<CurrentPlayerAddedDTO> player, ISubscriber<AddCardDTO> add, ISubscriber<SelectCardDTO> select, 
            IPublisher<CardMoveDTO> cardModePublisher, IPublisher<SelectViewCardDTO> selectViewCardPublisher, IPublisher<CanUseCardDTO> canUseCard, 
            IPublisher<UseCardDTO> useCard, ISubscriber<UseCardInputDTO> useCardInput, ISubscriber<UseCardReceiverDTO> useCardFinally, 
            IPublisher<CardDestroyViewDTO> cardDestroyPublisher, IPublisher<MoveUsedCardsDTO> moveUsedCardsPublisher, ISubscriber<UseCardOtherDTO> useCardOther,
            IPublisher<AddCardOtherDTO> addCard, IPublisher<PlayerSetCardCountDTO> playerSetCardCount, IPublisher<PlayerAddCardViewDTO> playerAddCardView)
        {
            Player = player;
            Add = add;
            Select = select;
            CardModePublisher = cardModePublisher;
            SelectView = selectViewCardPublisher;
            CanUseCard = canUseCard;
            UseCard = useCard;
            UseCardInput = useCardInput;
            UseCardFinally = useCardFinally;
            CardDestroyPublisher = cardDestroyPublisher;
            MoveUsedCardsPublisher = moveUsedCardsPublisher;
            UseCardOther = useCardOther;
            AddCard = addCard;
            PlayerSetCardCount = playerSetCardCount;
            PlayerAddCardView = playerAddCardView;
        }
    }
}