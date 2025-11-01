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
	    private CardData _cardData;

	    private IPublisher<CardMoveDTO> _cardMovePublisher;
	    private IPublisher<CardSetBombDTO> _cardSetBombPublisher;
	    private IPublisher<CardDestroyViewDTO> _cardDestroyPublisher;

	    public CardData CardData => _cardData;

	    [Inject]
	    private void Construct(IPublisher<CardMoveDTO> cardMovePublisher, IPublisher<CardSetBombDTO> cardSetBombPublisher)
	    {
		    _cardMovePublisher = cardMovePublisher;
		    _cardSetBombPublisher = cardSetBombPublisher;
	    }
	    
		public CardModel(CardData cardData)
		{
			_cardData = cardData;
		}

		public void MoveCard(Vector3 direction, float duration)
		{
			var dto = new CardMoveDTO(_cardData, direction, duration);
			_cardMovePublisher.Publish(dto);
		}

		public void SetBomb(bool active)
		{
			var dto = new CardSetBombDTO(_cardData, active);
			_cardSetBombPublisher.Publish(dto);
		}

		public void Dispose()
		{
			var dto = new CardDestroyViewDTO(_cardData);
			_cardDestroyPublisher.Publish(dto);
		}
    }
}