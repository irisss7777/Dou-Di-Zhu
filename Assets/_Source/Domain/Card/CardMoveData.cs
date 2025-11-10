using _Source.Contracts.Card;
using UnityEngine;

namespace _Source.Domain.Card
{
    public struct CardMoveData
    {
        public ICardModel CardModel { get; private set; }
        public Vector2 TargetPosition { get; private set; }

        public CardMoveData(ICardModel cardModel, Vector2 targetPosition)
        {
            CardModel = cardModel;
            TargetPosition = targetPosition;
        }
    }
}