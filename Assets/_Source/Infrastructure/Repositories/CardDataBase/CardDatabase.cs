using _Source.Contracts.Card;
using UnityEngine;

namespace _Source.Infrastructure.Repositories.CardDatabase
{
    [CreateAssetMenu(fileName = "CardDatabase", menuName = "ScriptableObjects/DataBase/Card/CardDatabase", order = 1)]
    public class CardDatabase : ScriptableObject
    {
        [SerializeField] private CardData[] _allCardsData;

        public int CardDataCount => _allCardsData.Length;

        public CardData GetGardData(int index)
        {
            return _allCardsData[index];
        }
    }
}