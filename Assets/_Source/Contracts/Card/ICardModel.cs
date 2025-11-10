using System;
using UnityEngine;

namespace _Source.Contracts.Card
{
    public interface ICardModel : IDisposable
    {
        public CardData CardData { get; }
        public void SetBomb(bool active);
        public void MoveCard(Vector3 direction, float duration);
        public void MoveCard(Vector3 direction, float duration, float scale);
    }
}