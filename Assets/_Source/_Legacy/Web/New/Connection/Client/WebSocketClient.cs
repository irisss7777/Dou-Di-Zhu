using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Web.New.Connection.Envelope;
using NativeWebSocket;

namespace Web.New.Connection.Client
{
    public class WebSocketClient : MonoBehaviour
    {
        public event Action<string> OnRawMessageReceived;
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<string> OnError;

        [SerializeField] private string serverBaseUrl = "wss://tma-game.ru/DouDiZhu/ws/";

        internal WebSocket socket;
        private bool isConnected = false;

        // Подключиться к серверу
        public async Task Connect() 
        {
            if (string.IsNullOrEmpty(serverBaseUrl))
            {
                Debug.LogError("WebSocketClient: serverBaseUrl не указан!");
                return;
            }

            string fullUrl = $"{serverBaseUrl}";
            socket = new WebSocket(fullUrl);

            socket.OnOpen += () =>
            {
                isConnected = true;
                Debug.Log("WebSocket соединение установлено");
                OnConnected?.Invoke();
            };

            socket.OnMessage += (bytes) =>
            {
                string msg = Encoding.UTF8.GetString(bytes);
                Debug.Log($"Получено сообщение: {msg}");
                OnRawMessageReceived?.Invoke(msg); // Отправляем сырое сообщение для дальнейшей обработки
            };

            socket.OnError += (err) =>
            {
                Debug.LogError($"WebSocket ошибка: {err}");
                OnError?.Invoke(err);
            };

            socket.OnClose += (code) =>
            {
                isConnected = false;
                Debug.LogWarning("WebSocket отключен");
                OnDisconnected?.Invoke();
            };

            await socket.Connect();
        }

        // Отправить событие и данные - теперь дженерик метод
        public void Send<T>(string eventName, T dataPayload)
        {
            if (socket == null || socket.State != WebSocketState.Open)
            {
                Debug.LogWarning($"WebSocket не готов для отправки. Событие: {eventName}");
                return;
            }

            var envelope = new WebSocketEnvelope<T> // Используем дженерик WebSocketEnvelope<T>
            {
                @event = eventName,
                data = dataPayload // data теперь является объектом T, а не JSON-строкой
            };

            string json = JsonUtility.ToJson(envelope);
            Debug.Log($"Отправка: {json}");
            socket.SendText(json);
        }

        private void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (socket != null)
            {
                socket.DispatchMessageQueue();
            }
#endif
        }

        private async void OnDestroy()
        {
            if (socket != null)
            {
                await socket.Close();
            }
        }
        
        // В WebSocketClient
        public async void CloseSocket()
        {
            if (socket?.State is WebSocketState.Open or WebSocketState.Connecting)
            {
                await socket.Close();
            }
        }
    }
}