using System;
using UnityEngine;

namespace _Source.Contracts.Card
{
    public struct CardData
    {
        public int CardValue { get; private set; }
        public int CardSuit { get; private set; }

        public CardData(int cardValue, int cardSuit)
        {
            CardValue = cardValue;
            CardSuit = cardSuit;
        }
    }
}