using System;
using System.Collections.Generic;
using _Source.Contracts.Card;
using Cysharp.Threading.Tasks;

namespace _Source.Contracts.CardHandler
{
    public interface ICardHandlerModel : IDisposable
    {
        public List<ICardModel> SelectedCardModels { get; }
        public List<ICardModel> CardModels { get; }
        public void InitCardGrid(ICardGridModel cardGridModel);
        public UniTask AddCards(ICardModel[] cardsModel);
        public bool SelectCard(CardData cardData, bool select);
        public void RemoveCard(CardData[] cardDatas);
        public void ReplaceOnGrid(ICardModel[] cards);
        public ICardModel GetCard(CardData cardData);
        public void ClearSelection();
    }
}