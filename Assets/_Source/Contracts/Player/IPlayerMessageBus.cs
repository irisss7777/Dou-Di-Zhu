using System;
using _Source.Contracts.DTO.Card;
using _Source.Contracts.DTO.Player;
using _Source.Contracts.DTO.Web;
using _Source.Contracts.DTO.Web.SendDTO;

namespace _Source.Contracts.Player
{
    public interface IPlayerMessageBus : IDisposable
    {
        public void PublishPlayerAdded(CurrentPlayerAddedDTO dto);
        public void PublishCardSelected(SelectViewCardDTO dto);
        public void PublishCanUseCard(CanUseCardDTO dto);
        public void PublishCardUsed(UseCardDTO dto);
        public void PublishCardsMoved(MoveUsedCardsDTO dto);
        public void PublishCardAddedToOther(AddCardOtherDTO dto);
        public void PublishCardCountUpdated(PlayerSetCardCountDTO dto);
        public void PublishCardViewAdded(PlayerAddCardViewDTO dto);
        
        public void SubscribeAddCard(Action<AddCardDTO> handler);
        public void SubscribeSelectCard(Action<SelectCardDTO> handler);
        public void SubscribeUseCardInput(Action<UseCardInputDTO> handler);
        public void SubscribeUseCardFinally(Action<UseCardReceiverDTO> handler);
        public void SubscribeUseCardOther(Action<UseCardOtherDTO> handler);
    }
}