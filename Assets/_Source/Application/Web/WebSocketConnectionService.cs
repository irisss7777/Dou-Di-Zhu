using System;
using System.Text;
using _Source.Contracts.DataBase;
using _Source.Contracts.DTO.Web;
using _Source.Contracts.DTO.Web.WebsocketConnectionDTO;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Zenject;
using NativeWebSocket;
using UnityEngine;


namespace _Source.Application.Web
{
    public class WebSocketConnectionService : IDisposable, ITickable, IInitializable
    {
        [Inject] private readonly IPublisher<ConnectionDTO> _connectionPublisher;
        [Inject] private readonly IPublisher<ErrorDTO> _errorPublisher;
        
        [Inject] private readonly IPublisher<RawMessageReceiveDTO> _rawMessagePublisher;

        [Inject] private readonly ISubscriber<InputConnectionDTO> _inputConnectionSubscriber;
        
        private string _serverBaseUrl;
        private WebSocket _socket;

        private WebMessageService _webMessageService;
        private WebReceiverService _webReceiverService;
        
        private DisposableBagBuilder _disposable;

        [Inject]
        private void Construct(IWebConfig webConfig, WebMessageService webMessageService, WebReceiverService webReceiverService)
        {
            _serverBaseUrl = webConfig.ServerBaseUrl;

            _webMessageService = webMessageService;
            _webReceiverService = webReceiverService;
        }

        public void Initialize()
        {
            _disposable = DisposableBag.CreateBuilder();
            _inputConnectionSubscriber.Subscribe((message) => UpdateConnection(message.Connection)).AddTo(_disposable);
        }

        private void UpdateConnection(bool connection)
        {
            if (connection)
                Connect().Forget();
            else
                Disconnect().Forget();
        }

        private void InitializeMessageServices()
        {
            _webMessageService.Initialize(_socket);
            _webReceiverService.Initialize();
        }

        public async UniTask Connect() 
        {
            if (string.IsNullOrEmpty(_serverBaseUrl))
                return;

            string fullUrl = $"{_serverBaseUrl}";
            _socket = new WebSocket(fullUrl);
            
            InitializeMessageServices();

            _socket.OnOpen += () => _connectionPublisher.Publish(new ConnectionDTO(true));

            _socket.OnMessage += (bytes) => _rawMessagePublisher.Publish(new RawMessageReceiveDTO(Encoding.UTF8.GetString(bytes)));

            _socket.OnError += (err) => _errorPublisher.Publish(new ErrorDTO(err));

            _socket.OnClose += (code) => _connectionPublisher.Publish(new ConnectionDTO(false));

            await _socket.Connect().AsUniTask();
        }

        public async UniTask Disconnect()
        {
            if (_socket?.State is WebSocketState.Open or WebSocketState.Connecting)
                await _socket.Close().AsUniTask();
        }

        public void Dispose()
        {
            Disconnect().Forget();
            _disposable.Build().Dispose();
            _webMessageService.Dispose();
            _webReceiverService.Dispose();
        }

        public void Tick()
        {
            #if !UNITY_WEBGL || UNITY_EDITOR
                if (_socket != null && _socket.State == WebSocketState.Open)
                {
                    _socket.DispatchMessageQueue();
                }
            #endif
        }
    }
}