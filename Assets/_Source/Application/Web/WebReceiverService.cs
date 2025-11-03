using System;
using System.Collections.Generic;
using _Source.Contracts.DTO.Web;
using _Source.Contracts.Web.Envelope;
using _Source.Contracts.Web.SocketMessage;
using MessagePipe;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Zenject;

namespace _Source.Application.Web
{
    public class WebReceiverService : IMessageHandler<RawMessageReceiveDTO>, IDisposable
    {
        private Dictionary<string, Action<RawMessageReceiveDTO>> _eventHandlers;
        
        [Inject] private readonly ISubscriber<RawMessageReceiveDTO> _inputConnectionDTO;
        
        [Inject] private readonly IPublisher<PlayerConnectedDTO> _connectedPlayerPublisher;
        [Inject] private readonly IPublisher<SidePlayerConnectedDTO> _newPlayerConnectedPublisher;
        [Inject] private readonly IPublisher<PlayerLeaveDTO> _playerLeavePublisher;
        [Inject] private readonly IPublisher<AllPlayerInfoDTO> _allPlayerInfoPublisher;
        [Inject] private readonly IPublisher<AddCardDTO> _addCardPublisher;

        private DisposableBagBuilder _disposable;
        
        private bool _isInitialized;

        public void Initialize()
        {
            if(_isInitialized)
                return;
            
            _eventHandlers = new Dictionary<string, Action<RawMessageReceiveDTO>>
            {
                { WebSocketMessageType.USER_JOIN.ToString(), CreateHandler(_connectedPlayerPublisher) },
                { WebSocketMessageType.NEW_USER_JOIN.ToString(), CreateHandler(_newPlayerConnectedPublisher) },
                { WebSocketMessageType.USER_LEAVE.ToString(), CreateHandler(_playerLeavePublisher) },
                { WebSocketMessageType.ALL_USER_INFO.ToString(), CreateHandler(_allPlayerInfoPublisher) },
                { WebSocketMessageType.ADD_CARD.ToString(), CreateHandler(_addCardPublisher) },
            };
            
            _disposable = DisposableBag.CreateBuilder();

            _inputConnectionDTO.Subscribe(this).AddTo(_disposable);

            _isInitialized = true;
        }
        
        
        public void Handle(RawMessageReceiveDTO message)
        {
            var tempEnvelope = JsonConvert.DeserializeObject<WebSocketEnvelopeData<JObject>>(message.Message);
            var type = tempEnvelope.Type;
            
            Debug.Log("[ Message| Received ] " + message.Message);
            
            if (_eventHandlers.TryGetValue(type, out var handler))  
                handler(message);
        }


        private Action<RawMessageReceiveDTO> CreateHandler<T>(IPublisher<T> publisher)
        {
            return message => publisher.Publish(JsonConvert.DeserializeObject<WebSocketEnvelopeData<T>>(message.Message).Data);
        }

        public void Dispose()
        {
            _disposable.Build().Dispose();
        }
    }
}