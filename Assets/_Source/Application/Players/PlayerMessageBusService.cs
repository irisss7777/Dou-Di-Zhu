using System;
using _Source.Contracts.DTO.Card;
using _Source.Contracts.DTO.Player;
using _Source.Contracts.DTO.Web;
using _Source.Contracts.DTO.Web.SendDTO;
using _Source.Contracts.Player;
using MessagePipe;
using Zenject;

namespace _Source.Application.Players
{
    public class PlayerMessageBusService : IPlayerMessageBus, IInitializable
    {
        [Inject] private readonly IPublisher<CurrentPlayerAddedDTO> _player;
        [Inject] private readonly ISubscriber<AddCardDTO> _add;
        [Inject] private readonly ISubscriber<SelectCardDTO> _select;
        [Inject] private readonly ISubscriber<UseCardInputDTO> _useCardInput;
        [Inject] private readonly ISubscriber<UseCardReceiverDTO> _useCardFinally;
        [Inject] private readonly ISubscriber<UseCardOtherDTO> _useCardOther;

        [Inject] private readonly IPublisher<CardMoveDTO> _cardModePublisher;
        [Inject] private readonly IPublisher<CardDestroyViewDTO> _cardDestroyPublisher;
        [Inject] private readonly IPublisher<MoveUsedCardsDTO> _moveUsedCardsPublisher;
        [Inject] private readonly IPublisher<PlayerSetCardCountDTO> _playerSetCardCount;
        [Inject] private readonly IPublisher<PlayerAddCardViewDTO> _playerAddCardView;

        [Inject] private readonly IPublisher<SelectViewCardDTO> _selectView;
        [Inject] private readonly IPublisher<CanUseCardDTO> _canUseCard;
        [Inject] private readonly IPublisher<UseCardDTO> _useCard;
        [Inject] private readonly IPublisher<AddCardOtherDTO> _addCard;
        
        private DisposableBagBuilder _disposable;

        public void Initialize() => _disposable = DisposableBag.CreateBuilder();
        
        public void PublishPlayerAdded(CurrentPlayerAddedDTO dto) => _player.Publish(dto);
        public void PublishCardSelected(SelectViewCardDTO dto) => _selectView.Publish(dto);
        public void PublishCanUseCard(CanUseCardDTO dto) => _canUseCard.Publish(dto);
        public void PublishCardUsed(UseCardDTO dto) =>  _useCard.Publish(dto);
        public void PublishCardsMoved(MoveUsedCardsDTO dto) => _moveUsedCardsPublisher.Publish(dto);
        public void PublishCardAddedToOther(AddCardOtherDTO dto) => _addCard.Publish(dto);
        public void PublishCardCountUpdated(PlayerSetCardCountDTO dto) =>  _playerSetCardCount.Publish(dto);
        public void PublishCardViewAdded(PlayerAddCardViewDTO dto) => _playerAddCardView.Publish(dto);

        public void SubscribeAddCard(Action<AddCardDTO> handler) => _add.Subscribe((message) => handler.Invoke(message)).AddTo(_disposable);
        public void SubscribeSelectCard(Action<SelectCardDTO> handler) => _select.Subscribe((message) => handler.Invoke(message)).AddTo(_disposable);
        public void SubscribeUseCardInput(Action<UseCardInputDTO> handler) => _useCardInput.Subscribe((message) => handler.Invoke(message)).AddTo(_disposable);
        public void SubscribeUseCardFinally(Action<UseCardReceiverDTO> handler) => _useCardFinally.Subscribe((message) => handler.Invoke(message)).AddTo(_disposable);
        public void SubscribeUseCardOther(Action<UseCardOtherDTO> handler) => _useCardOther.Subscribe((message) => handler.Invoke(message)).AddTo(_disposable);
        
        public void Dispose() => _disposable.Build().Dispose();
    }
}