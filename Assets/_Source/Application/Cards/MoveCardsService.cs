using System;
using System.Collections.Generic;
using System.Linq;
using _Source.Contracts.Card;
using _Source.Contracts.DataBase;
using _Source.Contracts.DTO.Card;
using _Source.Contracts.DTO.Web;
using _Source.Presentation.StartPositions;
using MessagePipe;
using UnityEngine;
using Zenject;

namespace _Source.Application.Cards
{
    public class MoveCardsService : IInitializable, IDisposable
    {
        private PlayerStartPositions _playerStartPositions;
        private ICardDataBase _cardDataBase;

        private List<ICardModel> _currentCards = new();
        private readonly Dictionary<string, List<ICardModel>> _usedCardsData = new();
        
        [Inject] private readonly ISubscriber<MoveUsedCardsDTO> _moveUsedCardsSubscriber;
        [Inject] private readonly ISubscriber<PlayerPassedDTO> _playerPassSubscriber;
        
        private DisposableBagBuilder _disposable;

        [Inject]
        private void Construct(PlayerStartPositions playerStartPositions, ICardDataBase cardDataBase)
        {
            _playerStartPositions = playerStartPositions;
            _cardDataBase = cardDataBase;
        }
        
        public void Initialize()
        {
            _disposable = DisposableBag.CreateBuilder();
            
            _moveUsedCardsSubscriber.Subscribe((message) => MoveUsedCards(message)).AddTo(_disposable);
            _playerPassSubscriber.Subscribe((message) => ClearUsedCard(message.UserName)).AddTo(_disposable);
        }

        public void MoveUsedCards(MoveUsedCardsDTO message)
        {
            string name = message.PlayerData.Name;
            ICardModel[] cardsModel = message.Cards;

            _currentCards = cardsModel.ToList();

            ClearUsedCard(name);
    
            PlayerViewPositionData playerViewPositionData = _playerStartPositions.GetData(name);
            Vector2 centerPosition = playerViewPositionData.UsedCardTransform.position;

            float spacing = _cardDataBase.DistanceBetweenUsedCards;
            float totalWidth = (_currentCards.Count - 1) * spacing;
            float startX = centerPosition.x - totalWidth / 2f;

            for (int i = 0; i < _currentCards.Count; i++)
            {
                float xPosition = startX + i * spacing;
                Vector2 targetPosition = new Vector2(xPosition, centerPosition.y);
        
                _currentCards[i].MoveCard(targetPosition, _cardDataBase.UseMoveDuration, _cardDataBase.UseMoveSize);
            }
            
            _usedCardsData.Add(name, _currentCards);
        }

        private void ClearUsedCard(string name)
        {
            if (_usedCardsData.ContainsKey(name))
            {
                foreach (var card in _usedCardsData[name])
                {
                    card.Dispose();
                }
            
                _usedCardsData.Remove(name);
            }
        }

        public void Dispose()
        {
            _disposable.Build().Dispose();
        }
    }
}