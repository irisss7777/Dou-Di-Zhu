using System;
using _Source.Contracts.Card;
using _Source.Contracts.DTO.Card;
using DG.Tweening;
using MessagePipe;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Source.Presentation.View.Card
{
    public class CardView : MonoBehaviour, ICardView, IPointerEnterHandler, IPointerExitHandler
    {
        private IPublisher<SelectInputCardDTO> _selectInputCardPublisher;
        
        private ISubscriber<SelectViewCardDTO> _selectViewCardSubscriber;
        private ISubscriber<CardMoveDTO> _cardModeSubscriber;
        private CardData _cardData;
        
        private DisposableBagBuilder _disposable;
        
        private bool _isSelected;
        
        public void Initialize(CardData cardData, ISubscriber<CardMoveDTO> cardMode, IPublisher<SelectInputCardDTO> selectInput, ISubscriber<SelectViewCardDTO> selectView)
        {
            _cardData = cardData;

            _selectInputCardPublisher = selectInput;

            _selectViewCardSubscriber = selectView;
            _cardModeSubscriber = cardMode;

            _disposable = DisposableBag.CreateBuilder();
            
            _cardModeSubscriber.Subscribe((message) => MoveView(message)).AddTo(_disposable);
            _selectViewCardSubscriber.Subscribe((message) => Select(message)).AddTo(_disposable);
        }

        private bool IsMyMessage(CardData data)
        {
            if(data.CardValue != _cardData.CardValue || data.CardSuit != _cardData.CardSuit)
                return false;

            return true;
        }

        private void MoveView(CardMoveDTO message)
        {
            if(!IsMyMessage(message.Data))
                return;
            
            transform.DOMove(message.Direction, message.Duration);
        }

        private void Select(SelectViewCardDTO message)
        {
            if (IsMyMessage(message.CardData))
            {
                _isSelected = message.Select;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _selectInputCardPublisher.Publish(new SelectInputCardDTO(_cardData, _isSelected, true));
        }
        
        private void OnDestroy()
        {
            _disposable.Build().Dispose();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _selectInputCardPublisher.Publish(new SelectInputCardDTO(_cardData, _isSelected, false));
        }
    }
}