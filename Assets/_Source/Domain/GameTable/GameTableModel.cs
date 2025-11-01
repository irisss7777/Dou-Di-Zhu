using System.Collections.Generic;
using _Source.Contracts.DTO.GameTable;
using _Source.Domain.Card;
using MessagePipe;
using Zenject;

namespace _Source.Domain.GameTable
{
    public class GameTableModel
    {
        private IPublisher<AddToTableDTO> _addToTablePublisher;
        private List<CardHandlerModel> _cardHandlerModels = new();

        [Inject]
        private void Construct(IPublisher<AddToTableDTO> addToTablePublisher)
        {
            _addToTablePublisher = addToTablePublisher;
        }

        public void AddCardHandlerOnTable(CardHandlerModel cardHandler)
        {
            _cardHandlerModels.Add(cardHandler);
            _addToTablePublisher.Publish(new AddToTableDTO(cardHandler));
        }
    }
}