using System;
using _Source.Contracts.DTO.Web.SendDTO;
using _Source.Contracts.Web.SocketMessage;
using MessagePipe;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Source.Controller.Test
{
    public class TestRegistrationPlayerController : MonoBehaviour
    {
        [Inject] private readonly IPublisher<PlayerConnectSendDTO> _playerConnectSendDTOSubscriber;

        [SerializeField] private Button _playerRegisterButton;
        [SerializeField] private InputField _playerNameField;

        private void Awake()
        {
            _playerRegisterButton.onClick.AddListener(OnRegisterPlayerClick);
        }

        private void OnRegisterPlayerClick()
        {
            if(_playerNameField.text == "")
                return;
            
            _playerConnectSendDTOSubscriber.Publish(new PlayerConnectSendDTO(new PlayerConnectedWebMessage(_playerNameField.text), WebSocketMessageType.USER_JOIN));
        }
    }
}