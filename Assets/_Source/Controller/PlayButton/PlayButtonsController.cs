using System;
using _Source.Contracts.DTO.Card;
using _Source.Contracts.DTO.Web;
using _Source.Contracts.DTO.Web.SendDTO;
using _Source.Contracts.Web.SocketMessage;
using MessagePipe;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Source.Controller.PlayButton
{
    public class PlayButtonsController : MonoBehaviour
    { 
        [Inject] private readonly IPublisher<PlayerPassDTO> _playerPassSubscriber;
        [Inject] private readonly IPublisher<UseCardInputDTO> _useCardInputSubscriber;
        
        [Inject] private readonly ISubscriber<CanUseCardReceiverDTO> _canUseCardReceiverSubscriber;

        [SerializeField] private Button _passButton;
        [SerializeField] private Button _playButton;
        
        private DisposableBagBuilder _disposable;

        private string _name;

        private void Start()
        {
            _passButton.onClick.AddListener(OnPassButtonClick);
            _playButton.onClick.AddListener(OnPlayButtonClick);
            
            _disposable = DisposableBag.CreateBuilder();
            
            _canUseCardReceiverSubscriber.Subscribe((message) => SetupPlayButtonActive(message.Can, message.UserName)).AddTo(_disposable);
        }
        
        private void OnPassButtonClick()
        {
            _playerPassSubscriber.Publish(new PlayerPassDTO(_name, WebSocketMessageType.USER_PASS));
        }

        private void OnPlayButtonClick()
        {
            _useCardInputSubscriber.Publish(new UseCardInputDTO());
        }
        
        private void SetupPlayButtonActive(bool active, string name)
        {
            _playButton.gameObject.SetActive(active);
            _name = name;
        }

        private void OnDestroy()
        {
            _disposable.Build().Dispose();
        }
    }
}