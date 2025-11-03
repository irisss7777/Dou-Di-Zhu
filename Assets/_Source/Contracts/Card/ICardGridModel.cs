using UnityEngine;

namespace _Source.Contracts.Card
{
    public interface ICardGridModel
    {
        public float Duration { get; }
        public Vector2 TryPlaceCard(ICardModel cardModel);
        public Vector2 GetSelectPosition(ICardModel cardModel, bool select);
    }
}