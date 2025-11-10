using System;
using _Source.Contracts.DTO.Player;
using _Source.Contracts.DTO.Web;
using _Source.Contracts.View;
using DG.Tweening;
using MessagePipe;
using TMPro;
using UnityEngine;

namespace _Source.Presentation.View.PlayerView
{
    public class PlayerView : MonoBehaviour, IPlayerView
    {
        private ISubscriber<PlayerSetCardCountDTO> _playerSetCardCountSubscriber;
        private ISubscriber<PlayerPassedDTO> _playerPassSubscriber;
        private ISubscriber<PlayerAddCardViewDTO> _playerAddCardViewSubscriber;
        
        [SerializeField] private CharacterAnimation _characterAnimation;
        [SerializeField] private GameObject _parentCharacterObject;
        [SerializeField] private GameObject _isMeObject;
        [SerializeField] private GameObject _infoPanelObject;
        [SerializeField] private GameObject _disconnectedObject;

        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _cardCountText;

        private TMP_Text _passText;

        private string _name;
        
        private DisposableBagBuilder _disposable;

        public void Initialize(ISubscriber<PlayerSetCardCountDTO> playerSetCardCount, ISubscriber<PlayerPassedDTO> playerPass, ISubscriber<PlayerAddCardViewDTO> playerAddCard)
        {
            _playerSetCardCountSubscriber = playerSetCardCount;
            _playerPassSubscriber = playerPass;
            _playerAddCardViewSubscriber = playerAddCard;

            _disposable = DisposableBag.CreateBuilder();
            
            _playerSetCardCountSubscriber.Subscribe((message) => SetupCardCount(message)).AddTo(_disposable);
            _playerPassSubscriber.Subscribe((message) => DisplayPass(message.UserName, true)).AddTo(_disposable);
            _playerAddCardViewSubscriber.Subscribe((message) => DisplayPass(message.Name, false)).AddTo(_disposable);
        }

        public void SetupCharacterSprite(Sprite[] newSprites, int direction, TMP_Text passText)
        {
            _characterAnimation.SetupSprite(newSprites);
            
            _parentCharacterObject.transform.localScale = new Vector2(
                _parentCharacterObject.transform.localScale.x * direction,
                _parentCharacterObject.transform.localScale.y);

            _passText = passText;
        }

        public void SetupTextInfo(string name, Vector2 position)
        {
            _name = name;
            _nameText.text = _name;
            _infoPanelObject.transform.localPosition = position;
        }

        public void DisplayPass(string name, bool active)
        {
            if(name != _name)
                return;
            
            float targetValue = active ? 1 : 0;
            _passText.DOFade(targetValue, 0.5f);
        }

        private void SetupCardCount(PlayerSetCardCountDTO message)
        {
            if (message.UserName == _name)
                _cardCountText.text = "" + message.CardCount;
        }
        
        public void SetIsMe()
        {
            _isMeObject.SetActive(true);
        }

        public void Leave(string name)
        {
            if (_name == name)
            {
                _disconnectedObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            _disposable.Build().Dispose();
        }
    }
}