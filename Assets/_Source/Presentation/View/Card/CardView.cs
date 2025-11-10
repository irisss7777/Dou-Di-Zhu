using System;
using _Source.Contracts.Card;
using _Source.Contracts.DTO.Card;
using DG.Tweening;
using MessagePipe;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Source.Presentation.View.Card
{
    public class CardView : MonoBehaviour, ICardView, IPointerEnterHandler, IPointerExitHandler
    {
        private IPublisher<SelectInputCardDTO> _selectInputCardPublisher;
        
        private ISubscriber<SelectViewCardDTO> _selectViewCardSubscriber;
        
        private ISubscriber<CardMoveDTO> _cardModeSubscriber;
        private ISubscriber<CardDestroyViewDTO> _cardDestroySubscriber;

        private CardData _cardData;
        
        private DisposableBagBuilder _disposable;

        [SerializeField] private SpriteRenderer _sprite;
        
        private bool _isSelected;
        
        public void Initialize(CardData cardData, ISubscriber<CardMoveDTO> cardMode, IPublisher<SelectInputCardDTO> selectInput,
            ISubscriber<SelectViewCardDTO> selectView, ISubscriber<CardDestroyViewDTO> cardDestroySubscriber)
        {
            _cardData = cardData;

            _selectInputCardPublisher = selectInput;

            _selectViewCardSubscriber = selectView;
            _cardModeSubscriber = cardMode;
            _cardDestroySubscriber = cardDestroySubscriber;

            _disposable = DisposableBag.CreateBuilder();
            
            _cardModeSubscriber.Subscribe((message) => MoveView(message)).AddTo(_disposable);
            _selectViewCardSubscriber.Subscribe((message) => Select(message)).AddTo(_disposable);
            _cardDestroySubscriber.Subscribe((message) => TryDestroy(message.Data));
        }

        public void SetupSprite(Sprite sprite)
        {
            _sprite.sprite = sprite;
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

            Sequence moveSequence = DOTween.Sequence();
            
            moveSequence.Join(transform.DOMove(message.Direction, message.Duration));
            moveSequence.Join(transform.DOScale(message.Scale, message.Duration));
            moveSequence.OnUpdate(() => UpdateSortingOrderByPosition());
        }
        
        private void UpdateSortingOrderByPosition()
        {
            int sortingOrder = Mathf.RoundToInt(transform.position.x * 100f);
            _sprite.sortingOrder = sortingOrder;
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

        public void OnPointerExit(PointerEventData eventData)
        {
            _selectInputCardPublisher.Publish(new SelectInputCardDTO(_cardData, _isSelected, false));
        }

        private void TryDestroy(CardData data)
        {
            if(data.CardSuit == _cardData.CardSuit && data.CardValue == _cardData.CardValue)
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            _disposable.Build().Dispose();
        }
    }
}