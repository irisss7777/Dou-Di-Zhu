using System;
using UnityEngine;

namespace _Source.Contracts.Card
{
    [Serializable]
    public struct CardData
    {
        [SerializeField] private int _numberInDeck; // номер карты в колоде, растасованной по порядку по мастям 
        [SerializeField] private int _numberInSuit; // номер карты в массиве карт своей масти (3-0, 4-1, 5-2, 6-3, 7-4, 8-5, 9-6, 10-7, J-8, Q-9, K-10, A-11, 2-12), если масть джокеров, то BlackJoker-0, RedJoker-1
        [SerializeField] private Suits _suit; // clubs, diamonds, hearts, spades, jokers 
        [SerializeField] private int _value; //значение, написанное на самой карте (A-1, 2-2, 3-3, 4-4, 5-5, 6-6, 7-7, 8-8, 9-9, 10-10, J-11, Q-12, K-13), если масть джокеров, то BlackJoker-1, RedJoker-2

        public int NumberInDeck => _numberInDeck;
        public int NumberInSuit => _numberInSuit;
        public Suits Suit => _suit;
        public int Value => _value;

        public bool Equals(CardData data)
        {
            if (data.NumberInDeck == _numberInDeck && 
                data.NumberInSuit == _numberInSuit && 
                data.Suit == _suit && 
                data.Value == _value)
                return true;

            return false;
        }
    }
}