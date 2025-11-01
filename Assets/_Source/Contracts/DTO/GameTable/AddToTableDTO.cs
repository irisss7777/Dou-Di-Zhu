using _Source.Contracts.CardHandler;

namespace _Source.Contracts.DTO.GameTable
{
    public struct AddToTableDTO
    {
        public ICardHandlerModel CardHandlerModel { get; private set; }

        public AddToTableDTO(ICardHandlerModel cardHandlerModel)
        {
            CardHandlerModel = cardHandlerModel;
        }
    }
}