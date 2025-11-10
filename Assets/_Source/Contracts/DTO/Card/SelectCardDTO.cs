using _Source.Contracts.Card;
using _Source.Contracts.DTO.Web;

namespace _Source.Contracts.DTO.Card
{
    public struct SelectCardDTO
    {
        public CardData CardData { get; private set; }
        public bool Select { get; private set; }
        public string UserID { get; private set; }

        public SelectCardDTO(CardData cardData, bool select, string userID)
        {
            CardData = cardData;
            Select = select;
            UserID = userID;
        }
    }
}