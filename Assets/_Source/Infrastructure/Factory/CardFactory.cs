using System;
using _Source.Contracts.Card;
using _Source.Contracts.DataBase;
using _Source.Contracts.DTO.Card;
using _Source.Contracts.DTO.Web;
using _Source.Contracts.Factory;
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
    public class CardFactory : IDisposable, IInitializable, ICardFactory
    {
        private CardDatabase _cardDatabase;
        private CardStartPosition _cardStartPosition;
        private PlayerStartPositions _playerStartPositions;
        
        [Inject] private readonly IPublisher<SelectInputCardDTO> _selectInputCardPublisher;
        
        [Inject] private readonly ISubscriber<SelectViewCardDTO> _selectViewCardPublisher;
        [Inject] private readonly ISubscriber<AddCardDTO> _addCardPublisher;
        [Inject] private readonly ISubscriber<AddCardOtherDTO> _addCardOtherPublisher;
        [Inject] private readonly ISubscriber<CardMoveDTO> _cardModeSubscriber;
        [Inject] private readonly ISubscriber<CardDestroyViewDTO> _cardDestroySubscriber;
        
        [Inject] private readonly IPublisher<CardMoveDTO> _cardModePublisher;
        [Inject] private readonly IPublisher<CardDestroyViewDTO> _cardDestroyPublisher;
        
        private DisposableBagBuilder _disposable;
        
        [Inject]
        private void Construct(ICardDataBase cardDatabase, CardStartPosition cardStartPosition, PlayerStartPositions playerStartPositions)
        {
            _cardDatabase = cardDatabase as CardDatabase;
            _cardStartPosition = cardStartPosition;

            _playerStartPositions = playerStartPositions;
        }
        
        public void Initialize()
        {
            _disposable = DisposableBag.CreateBuilder();
            
            _addCardPublisher.Subscribe((message) => CreateCard(message.CardData)).AddTo(_disposable);
            _addCardOtherPublisher.Subscribe((message) => CreateCardOther(message)).AddTo(_disposable);
        }

        public ICardModel CreateCardModel(CardData cardData)
        {
            return new CardModel(cardData, _cardModePublisher, _cardDestroyPublisher);
        }

        private void CreateCard(CardData[] cardData)
        {
            foreach (var card in cardData)
            {
                CardView cardPrefab = _cardDatabase.CardPrefab as CardView;
                
                CardView cardView = GameObject.Instantiate(cardPrefab, _cardStartPosition.gameObject.transform.position, Quaternion.identity);
                
                cardView.Initialize(card, _cardModeSubscriber, _selectInputCardPublisher, _selectViewCardPublisher, _cardDestroySubscriber);
                cardView.SetupSprite(_cardDatabase.GetCardSprite(card));
            }
        }
        
        private void CreateCardOther(AddCardOtherDTO message)
        {
            foreach (var card in message.CardData)
            {
                CardView cardPrefab = _cardDatabase.CardPrefab as CardView;
                
                CardView cardView = GameObject.Instantiate(cardPrefab, _playerStartPositions.GetData(message.UserName).Transform.position, Quaternion.identity);
                
                cardView.Initialize(card, _cardModeSubscriber, _selectInputCardPublisher, _selectViewCardPublisher, _cardDestroySubscriber);
                cardView.SetupSprite(_cardDatabase.GetCardSprite(card));
            }
        }

        public void Dispose()
        {
            _disposable.Build().Dispose();
        }
    }
}