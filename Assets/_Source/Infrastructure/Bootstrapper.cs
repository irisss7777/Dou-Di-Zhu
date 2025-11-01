using _Source.Contracts.DTO.Web;
using MessagePipe;
using UnityEngine;
using Zenject;

namespace _Source.Infrastructure
{
    public class Bootstrapper : MonoBehaviour
    {
        [Inject] private readonly IPublisher<InputConnectionDTO> _playerConnectSendDTOSubscriber;
        
        private void Start()
        {
            _playerConnectSendDTOSubscriber.Publish(new InputConnectionDTO(true));
        }
    }
}