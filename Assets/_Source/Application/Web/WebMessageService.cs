using System;
using _Source.Contracts.DTO.Web.SendDTO;
using _Source.Contracts.Web.Envelope;
using _Source.Contracts.Web.SocketMessage;
using MessagePipe;
using Newtonsoft.Json;
using Zenject;
using NativeWebSocket;
using UnityEngine;

namespace _Source.Application.Web
{
    public class WebMessageService : IDisposable
    {
        private WebSocket _socket;
        
        [Inject] private readonly ISubscriber<PlayerConnectSendDTO> _playerConnectSendDTOSubscriber;
        
        private DisposableBagBuilder _disposable;

        private bool _isInitialized;
        
        public void Initialize(WebSocket socket)
        {
            _socket = socket;
                
            if(_isInitialized)
                return;
            
            _disposable = DisposableBag.CreateBuilder();
            
            _playerConnectSendDTOSubscriber.Subscribe((message ) => SendMessage(message.Type, message.Data)).AddTo(_disposable);
            
            _isInitialized = true;
        }

        private void SendMessage<T>(WebSocketMessageType type, T data)
        {
            if (_socket == null || _socket.State != WebSocketState.Open)  
                return;
            
            var envelope = new WebSocketEnvelopeData<T>(data, type);

            string json = JsonConvert.SerializeObject(envelope);
            Debug.Log("[ Message| Send ] " + json);
            _socket.SendText(json);
        }

        public void Dispose()
        {
            _disposable.Build().Dispose();
        }
    }
}