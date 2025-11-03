using System;
using _Source.Contracts.Card;
using _Source.Contracts.DTO.Card;
using MessagePipe;
using UnityEngine;
using Zenject;

namespace _Source.Domain.Card
{
    public class CardModel : ICardModel
    {
	    private IPublisher<CardMoveDTO> _cardModePublisher;
	    private CardData _cardData;

	    public CardData CardData => _cardData;
	    
		public CardModel(CardData cardData, IPublisher<CardMoveDTO> cardModePublisher)
		{
			_cardModePublisher = cardModePublisher;
			_cardData = cardData;
		}

		public void MoveCard(Vector3 direction, float duration)
		{
			var dto = new CardMoveDTO(_cardData, direction, duration);
			_cardModePublisher.Publish(dto);
		}

		public void SetBomb(bool active)
		{
			var dto = new CardSetBombDTO(_cardData, active);
		}

		public void Dispose()
		{
			var dto = new CardDestroyViewDTO(_cardData);
		}
    }
}