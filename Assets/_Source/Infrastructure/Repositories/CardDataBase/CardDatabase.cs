using System.Linq;
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
        [SerializeField] private CardConfig[] _allCardsData;
        
        [Header("UseMove")]
        [SerializeField] private float _useMoveDuration;
        [SerializeField] private float _distanceBetweenUsedCards;
        [SerializeField] private float _useMoveSize;

        public float Duration => _moveDuration;
        public float UseMoveDuration => _useMoveDuration;
        public int SlotsCount => _slotsCount;
        public Vector2 SelectDeltaPosition => _selectDeltaPosition;
        public float DistanceBetweenCards => _distanceBetweenCards;
        public float DistanceBetweenUsedCards => _distanceBetweenUsedCards;
        public Vector2 DefualtCardSlotPosition => _defaultCardPositionInSlot;
        public int CardDataCount => _allCardsData.Length;
        public ICardView CardPrefab => _cardViewPrefab;
        public float UseMoveSize => _useMoveSize;

        public Sprite GetCardSprite(CardData cardData)
        {
            return _allCardsData.First(x => x.CardValue == cardData.CardValue && x.CardSuit == cardData.CardSuit)
                .Sprite;
        }
    }
}