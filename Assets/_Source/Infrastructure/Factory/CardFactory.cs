using System;
using _Source.Contracts.Card;
using _Source.Contracts.DataBase;
using _Source.Contracts.DTO.Card;
using _Source.Contracts.DTO.Web;
using _Source.Controller.Input;
using _Source.Domain.Card;
using Zenject;
using _Source.Infrastructure.Repositories.CardDatabase;
using _Source.Presentation.StartPositions;
using _Source.Presentation.View.Card;
using MessagePipe;
using UnityEngine;

namespace _Source.Infrastructure.Factory
{
    public class CardFactory : IDisposable, IInitializable
    {
        private CardDatabase _cardDatabase;
        private CardStartPosition _cardStartPosition;
        private ClickInputController _clickInputController;
        
        [Inject] private readonly IPublisher<SelectInputCardDTO> _selectInputCardPublisher;
        
        [Inject] private readonly ISubscriber<SelectViewCardDTO> _selectViewCardPublisher;
        [Inject] private readonly ISubscriber<AddCardDTO> _addCardPublisher;
        [Inject] private readonly ISubscriber<CardMoveDTO> _cardModeSubscriber;
        
        private DisposableBagBuilder _disposable;
        
        [Inject]
        private void Construct(ICardDataBase cardDatabase, CardStartPosition cardStartPosition, ClickInputController clickInputController)
        {
            _cardDatabase = cardDatabase as CardDatabase;
            _cardStartPosition = cardStartPosition;

            _clickInputController = clickInputController;
        }
        
        public void Initialize()
        {
            _disposable = DisposableBag.CreateBuilder();
            
            _addCardPublisher.Subscribe((message) => CreateCard(message.CardData)).AddTo(_disposable);
        }

        public void CreateCard(CardData[] cardData)
        {
            foreach (var card in cardData)
            {
                CardView cardPrefab = _cardDatabase.CardPrefab as CardView;
                CardView cardView = GameObject.Instantiate(cardPrefab, _cardStartPosition.gameObject.transform.position, Quaternion.identity);
                cardView.Initialize(card, _cardModeSubscriber, _selectInputCardPublisher, _selectViewCardPublisher);
            }
        }

        public void Dispose()
        {
            _disposable.Build().Dispose();
        }
    }
}