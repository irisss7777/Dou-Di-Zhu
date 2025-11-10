using System.Collections.Generic;
using System.Linq;
using _Source.Contracts.Card;
using _Source.Contracts.DataBase;
using UnityEngine;
using Zenject;

namespace _Source.Domain.Card
{
    public class CardGridModel : ICardGridModel, IInitializable
    {
        private List<CardSlot> _cardsSlots = new();
        private Dictionary<int, List<CardSlot>> _groupSlots = new();
        private float _duration;
        public float Duration => _duration;

        private ICardDataBase _cardDataBase;

        [Inject]
        private void Construct(ICardDataBase cardDataBase)
        {
            _cardDataBase = cardDataBase;
            _duration = cardDataBase.Duration;
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

            for (int i = 0; i < slotsCount; i++)
            {
                float positionX = (i - (slotsCount - 1) / 2f) * distanceBetweenSlots;
                _cardsSlots.Add(new CardSlot(new Vector2(positionX, defaultPosition.y)));
            }
            
            _cardsSlots = _cardsSlots.OrderBy(slot => slot.CardPosition.x).ToList();
        }

        public Vector2 TryPlaceCard(ICardModel cardModel)
        {
            return GetBestPositionForCard(cardModel);
        }

        private Vector2 GetBestPositionForCard(ICardModel cardModel)
        {
            int cardValue = cardModel.CardData.CardValue;
            
            if (_groupSlots.ContainsKey(cardValue) && _groupSlots[cardValue].Count > 0)
            {
                var group = _groupSlots[cardValue];
                var firstSlot = group.First();
                int firstIndex = _cardsSlots.IndexOf(firstSlot);
                
                for (int offset = 1; offset < _cardsSlots.Count; offset++)
                {
                    if (firstIndex + offset < _cardsSlots.Count)
                    {
                        var rightSlot = _cardsSlots[firstIndex + offset];
                        if (!rightSlot.HasCard())
                        {
                            rightSlot.AddCard(cardModel);
                            group.Add(rightSlot);
                            return rightSlot.CardPosition;
                        }
                    }

                    if (firstIndex - offset >= 0)
                    {
                        var leftSlot = _cardsSlots[firstIndex - offset];
                        if (!leftSlot.HasCard())
                        {
                            leftSlot.AddCard(cardModel);
                            group.Add(leftSlot);
                            return leftSlot.CardPosition;
                        }
                    }
                }
            }

            foreach (var slot in _cardsSlots)
            {
                if (!slot.HasCard())
                {
                    slot.AddCard(cardModel);

                    if (!_groupSlots.ContainsKey(cardValue))
                        _groupSlots[cardValue] = new List<CardSlot>();
                    
                    _groupSlots[cardValue].Add(slot);
                    return slot.CardPosition;
                }
            }
            
            return Vector2.zero;
        }

        public Vector2 GetSelectPosition(ICardModel cardModel, bool select)
        {
            if (cardModel == null)
                return Vector2.zero;

            int foundSlots = 0;
            CardSlot foundSlot = null;
    
            foreach (var slot in _cardsSlots)
            {
                if (slot.CardModel == cardModel)
                {
                    foundSlot = slot;
                    foundSlots++;
                }
            }

            if (foundSlots == 0)
                return Vector2.zero;

            Vector2 basePosition = foundSlot.CardPosition;
            Vector2 selectedPosition = basePosition + _cardDataBase.SelectDeltaPosition;

            return select ? selectedPosition : basePosition;
        }

        public void ClearGrid()
        {
            foreach (var cardSlot in _cardsSlots)
            {
                cardSlot.ClearSlot();
            }
            _groupSlots.Clear();
        }

        public void RearrangeCardsWithGrouping(ICardModel[] cards)
        {
            ClearGrid();
            
            if (cards == null || cards.Length == 0) return;

            var cardGroups = cards
                .GroupBy(card => card.CardData.CardValue)
                .OrderBy(group => group.Key)
                .ToList();

            var sortedCards = cardGroups
                .SelectMany(group => group.OrderBy(card => card.CardData.CardSuit))
                .ToArray();

            int totalCards = sortedCards.Length;
            int totalSlots = _cardsSlots.Count;

            int startIndex = (totalSlots - totalCards) / 2;
            startIndex = Mathf.Max(0, startIndex);

            for (int i = 0; i < totalCards && startIndex + i < totalSlots; i++)
            {
                var slot = _cardsSlots[startIndex + i];
                var card = sortedCards[i];
                slot.AddCard(card);
 
                int cardValue = card.CardData.CardValue;
                if (!_groupSlots.ContainsKey(cardValue))
                    _groupSlots[cardValue] = new List<CardSlot>();
                
                _groupSlots[cardValue].Add(slot);
            }
            
            OptimizeGroupPlacement();
        }

        private void OptimizeGroupPlacement()
        {
            var groups = _groupSlots
                .Where(kvp => kvp.Value.Count > 0)
                .OrderBy(kvp => kvp.Key)
                .ToList();

            var tempSlots = new List<CardSlot>();
            var tempCards = new List<ICardModel>();

            foreach (var group in groups)
            {
                var sortedGroupSlots = group.Value.OrderBy(slot => _cardsSlots.IndexOf(slot)).ToList();
                foreach (var slot in sortedGroupSlots)
                {
                    if (slot.HasCard())
                    {
                        tempCards.Add(slot.CardModel);
                        tempSlots.Add(slot);
                    }
                }
            }

            ClearGrid();

            int centerIndex = (_cardsSlots.Count - tempCards.Count) / 2;
            centerIndex = Mathf.Max(0, centerIndex);
            
            for (int i = 0; i < tempCards.Count && centerIndex + i < _cardsSlots.Count; i++)
            {
                var slot = _cardsSlots[centerIndex + i];
                var card = tempCards[i];
                slot.AddCard(card);

                int cardValue = card.CardData.CardValue;
                if (!_groupSlots.ContainsKey(cardValue))
                    _groupSlots[cardValue] = new List<CardSlot>();
                
                _groupSlots[cardValue].Add(slot);
            }
        }
    }
}