using _Source.Contracts.DTO.Web;
using TMPro;
using MessagePipe;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Source.Controller.Test
{
    public class TestConnectionController : MonoBehaviour
    {

        [Inject] private readonly IPublisher<InputConnectionDTO> _playerConnectSendDTOSubscriber;
        
        private DisposableBagBuilder _disposable;

        [SerializeField] private Button _playerConnectButton;
        [SerializeField] private TMP_Text _playerConnectText;

        private bool _isConnected;

        private void Awake()
        {
            _disposable = DisposableBag.CreateBuilder();
            
            _playerConnectButton.onClick.AddListener(OnConnectButtonClick);
        }

        private void OnConnectButtonClick()
        {
            _playerConnectSendDTOSubscriber.Publish(new InputConnectionDTO(!_isConnected));
        }

        private void OnDestroy()
        {
            _disposable.Build().Dispose();
        }
    }
}