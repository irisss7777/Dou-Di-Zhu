using _Source.Contracts.Card;
using _Source.Contracts.DataBase;
using _Source.Presentation.View.Card;
using UnityEngine;

namespace _Source.Infrastructure.Repositories.CardDatabase
{
    [CreateAssetMenu(fileName = "CardDatabase", menuName = "ScriptableObjects/DataBase/Card/CardDatabase", order = 1)]
    public class CardDatabase : ScriptableObject, ICardDataBase
    {
        [SerializeField] private int _slotsCount;
        [SerializeField] private float _distanceBetweenCards;
        [SerializeField] private Vector2 _selectDeltaPosition;
        [SerializeField] private Vector2 _defaultCardPositionInSlot;
        [SerializeField] private float _moveDuration;
        [SerializeField] private CardView _cardViewPrefab;
        [SerializeField] private CardData[] _allCardsData;

        public float Duration => _moveDuration;
        public int SlotsCount => _slotsCount;
        public Vector2 SelectDeltaPosition => _selectDeltaPosition;
        public float DistanceBetweenCards => _distanceBetweenCards;
        public Vector2 DefualtCardSlotPosition => _defaultCardPositionInSlot;
        public int CardDataCount => _allCardsData.Length;
        public ICardView CardPrefab => _cardViewPrefab;

        public CardData GetGardData(int index)
        {
            return _allCardsData[index];
        }
    }
}