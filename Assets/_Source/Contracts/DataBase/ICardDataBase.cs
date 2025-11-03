using _Source.Contracts.Card;
using UnityEngine;

namespace _Source.Contracts.DataBase
{
    public interface ICardDataBase
    {
        public float Duration { get; }
        public int SlotsCount { get; }
        public Vector2 SelectDeltaPosition { get; }
        public float DistanceBetweenCards { get; }
        public Vector2 DefualtCardSlotPosition { get; }
        public int CardDataCount { get; }
        public ICardView CardPrefab { get; }
    }
}