using _Source.Contracts.Card;
using UnityEngine;

namespace _Source.Contracts.DTO.Card
{
    public struct CardMoveDTO
    {
        public CardData Data { get; private set; }
        public Vector3 Direction { get; private set; }
        public float Duration { get; private set; }

        public CardMoveDTO(CardData data, Vector3 direction, float duration)
        {
            Data = data;
            Direction = direction;
            Duration = duration;
        }
    }
}