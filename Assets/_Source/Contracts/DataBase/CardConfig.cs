using System;
using UnityEngine;

namespace _Source.Contracts.DataBase
{
    [Serializable]
    public struct CardConfig
    {
        public int CardValue;
        public int CardSuit;
        public Sprite Sprite;
    }
}