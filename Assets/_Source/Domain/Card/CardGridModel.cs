using System.Collections.Generic;
using _Source.Contracts.Card;
using _Source.Contracts.DataBase;
using UnityEngine;
using Zenject;

namespace _Source.Domain.Card
{
    public class CardGridModel : ICardGridModel, IInitializable
    {
        private List<CardSlot> _cardsSlots = new();

        private float _duration;
        public float Duration => _duration;

        private ICardDataBase _cardDataBase;

        [Inject]
        private void Construct(ICardDataBase cardDataBase)
        {
            _cardDataBase = cardDataBase;
            _duration = _cardDataBase.Duration;
        }
        
        public void Initialize()
        {
            InitializeGrid(_cardDataBase);
        }

        private void InitializeGrid(ICardDataBase cardDataBase)
        {
            int slotsCount = cardDataBase.SlotsCount;
            Vector2 defaultPosition = cardDataBase.DefualtCardSlotPosition;
            float distanceBetweenSlots = cardDataBase.DistanceBetweenCards;
            
            List<float> allPositions = new();

            for (int i = 1; i < slotsCount / 2; i++)
            {
                allPositions.Add(distanceBetweenSlots * i);
                allPositions.Add(-distanceBetweenSlots * i);
            }
            
            allPositions.Add(defaultPosition.x);
            
            allPositions.Sort();

            foreach (var position in allPositions)
            {
                _cardsSlots.Add(new CardSlot(new Vector2(position, defaultPosition.y)));
            }
        }

        public Vector2 TryPlaceCard(ICardModel cardModel)
        {
            Vector2 position = Vector2.zero;
            
            foreach (var cardSlot in _cardsSlots)
            {
                if (!cardSlot.HasCard())
                {
                    cardSlot.AddCard(cardModel);
                    position = cardSlot.CardPosition;
                    break;
                }
            }

            return position;
        }

        public Vector2 GetSelectPosition(ICardModel cardModel, bool select)
        {
            Vector2 position = Vector2.zero;
            
            foreach (var cardSlot in _cardsSlots)
            {
                if (cardSlot.HasCard())
                {
                    if (cardSlot.CardModel == cardModel)
                    {
                        if(select)
                            position = cardSlot.CardPosition + _cardDataBase.SelectDeltaPosition;
                        else
                            position = cardSlot.CardPosition;
                        
                        break;
                    }
                }
            }

            return position;
        }
    }
}