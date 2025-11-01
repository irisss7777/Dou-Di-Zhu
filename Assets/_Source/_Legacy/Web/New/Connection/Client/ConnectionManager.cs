using System;
using System.Collections;
using UnityEngine;
using Web.New.Connection.Envelope;
using Web.New.Messages.Payloads.ClientServer;
using Web.New.Messages.Payloads.ServerClient;

namespace Web.New.Connection.Client
{
    public class ConnectionManager : MonoBehaviour
    {
        [SerializeField] private WebSocketClient wsClient;
        public ConnectPayload connectionProps;
        private Coroutine pingPongCoroutine; // Одна корутина для пинга и проверки понг
        private bool isConnectedToSocket = false; // Состояние подключения к сокету
        private float lastPongReceivedTime;
        private const float PING_INTERVAL = 30f; // Сервер ждет пинг от клиента 1 минуту, клиент отправляет каждые 30 секунд
        private const float PONG_RECEIVE_TIMEOUT = 65f; // Если не получили понг за 65 секунд, считаем дисконнект (с запасом)

        public event Action OnConnectionLost;

        public void Initialize()
        {
            wsClient.OnConnected += OnSocketConnected;
            wsClient.OnDisconnected += OnSocketDisconnected;
            wsClient.OnError += OnSocketError;
            wsClient.OnRawMessageReceived += HandleIncomingRawMessage;
            Debug.Log("ConnectionManager инициализирован.");
        }

        // Начинает подключение к WebSocket и отправляет событие "connect".
        public void StartGameConnection(ConnectPayload connectionProps)
        {
            this.connectionProps = connectionProps;

            StartCoroutine(ConnectAndConnectEventRoutine());
        }

        private IEnumerator ConnectAndConnectEventRoutine()
        {
            Debug.Log("ConnectionManager: Попытка подключения к сокету...");
            yield return wsClient.Connect();

            if (isConnectedToSocket)
            {
                Debug.Log("ConnectionManager: Сокет подключен, отправляем 'connect' событие.");
                // Отправляем событие "connect" только после успешного подключения сокета
                wsClient.Send("connect", connectionProps);
                lastPongReceivedTime = Time.time; // Сбрасываем таймер понг
                StartPingPongLoop();
            }
            else
            {
                Debug.LogError("ConnectionManager: Не удалось подключиться к сокету.");
                // Здесь можно обработать ошибку подключения, например, показать сообщение пользователю
            }
        }

        // Отключается от лобби и закрывает соединение.
        public void DisconnectGameConnection()
        {
            if (wsClient != null && isConnectedToSocket)
            {
                Debug.Log("ConnectionManager: Отправляем 'disconnect' и закрываем соединение.");
                wsClient.Send("disconnect", new DisconnectPayload { lobby_id = connectionProps.lobby_id, user_id = connectionProps.user_id });
                wsClient.gameObject.GetComponent<WebSocketClient>().CloseSocket();
            }
            StopPingPongLoop();
            isConnectedToSocket = false;
        }

        private void OnSocketConnected()
        {
            isConnectedToSocket = true;
            Debug.Log("ConnectionManager: Сокет подключен, отправляем 'connect' событие.");
            // Отправляем событие "connect" только после успешного подключения сокета
            wsClient.Send("connect", connectionProps);
            lastPongReceivedTime = Time.time; // Сбрасываем таймер понг
            StartPingPongLoop();
        }

        private void OnSocketDisconnected()
        {
            isConnectedToSocket = false;
            Debug.LogWarning("ConnectionManager: Сокет отключен, попытка переподключения...");
            StopPingPongLoop();
            StartCoroutine(ReconnectRoutine());
        }

        private void OnSocketError(string error)
        {
            Debug.LogError("ConnectionManager: ошибка WS - " + error);
            // OnError?.Invoke(error); // Можно пробросить наружу, если нужно
        }

        // Обработка входящих сообщений для пинг-понга
        private void HandleIncomingRawMessage(string rawMessage)
        {
            // Здесь нам нужно распарсить только event, чтобы понять, это "ping" или "pong"
            try
            {
                var tempEnvelope = JsonUtility.FromJson<WebSocketEnvelope<object>>(rawMessage);
                if (tempEnvelope != null)
                {
                    if (tempEnvelope.@event == "pong")
                    {
                        lastPongReceivedTime = Time.time;
                        Debug.Log("ConnectionManager: Получен pong от сервера.");
                    }
                    else if (tempEnvelope.@event == "ping")
                    {
                        // Отвечаем на ping от сервера
                        var pingPayload = JsonUtility.FromJson<WebSocketEnvelope<PingPayload>>(rawMessage).data;
                        wsClient.Send("pong", new PongPayload { timestamp = pingPayload.timestamp });
                        Debug.Log("ConnectionManager: Отправлен pong в ответ на серверный ping.");
                        lastPongReceivedTime = Time.time; // Считаем, что это тоже признак активности
                    }
                    // MessageDispatcher обработает все остальные события
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"ConnectionManager: Ошибка при парсинге сообщения для пинг-понга: {ex.Message} -> {rawMessage}");
            }
        }


        private IEnumerator ReconnectRoutine()
        {
            yield return new WaitForSeconds(2); // Задержка перед переподключением
            if (!isConnectedToSocket) // Если еще не переподключились
            {
                Debug.Log("ConnectionManager: Попытка переподключения...");
                // Пытаемся подключиться с теми же параметрами
                StartGameConnection(connectionProps); // Пример получения токена
            }
        }

        private void StartPingPongLoop()
        {
            if (pingPongCoroutine == null)
                pingPongCoroutine = StartCoroutine(PingPongLoop());
        }

        private void StopPingPongLoop()
        {
            if (pingPongCoroutine != null)
                StopCoroutine(pingPongCoroutine);
            pingPongCoroutine = null;
        }

        private IEnumerator PingPongLoop()
        {
            while (isConnectedToSocket)
            {
                // Отправляем ping клиентом
                yield return new WaitForSeconds(PING_INTERVAL);
                if (wsClient != null && wsClient.socket.State == NativeWebSocket.WebSocketState.Open) // Проверка на null и состояние сокета
                {
                    wsClient.Send("ping", new PingPayload { timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() });
                    Debug.Log("ConnectionManager: Отправлен ping клиентом.");
                }

                // Проверяем таймаут получения pong
                if (Time.time - lastPongReceivedTime > PONG_RECEIVE_TIMEOUT)
                {
                    Debug.LogWarning("ConnectionManager: Таймаут получения pong. Отключаем.");
                    OnConnectionLost?.Invoke(); // Уведомляем другие части игры о потере соединения
                    DisconnectGameConnection(); // Принудительное отключение
                    yield break; // Выходим из корутины
                }
            }
        }

        private void OnDestroy()
        {
            StopPingPongLoop();
            if (wsClient != null)
            {
                wsClient.OnConnected -= OnSocketConnected;
                wsClient.OnDisconnected -= OnSocketDisconnected;
                wsClient.OnError -= OnSocketError;
                wsClient.OnRawMessageReceived -= HandleIncomingRawMessage;
            }
        }

        public void SendPlayerReady() =>
            wsClient.Send("player_ready", new PlayerReadyPayload { lobby_id = connectionProps.lobby_id, user_id = connectionProps.user_id });

        public void SendGotGameState() =>
            wsClient.Send("got_game_state", new GotGameStatePayload { lobby_id = connectionProps.lobby_id, user_id = connectionProps.user_id });

        public void SendPlayLLS(int bid) =>
            wsClient.Send("play_lls", new PlayLLSPayload { lobby_id = connectionProps.lobby_id, user_id = connectionProps.user_id, bid = bid });

        public void SendPlay(long userId, int[] cards, ComboType comboType) =>
            wsClient.Send("play", new PlayPayload { lobby_id = connectionProps.lobby_id, user_id = userId, cards = cards, combo_type = comboType.ToString()});


        public void SendPass(long userId) =>
            wsClient.Send("pass", new PassPayload { lobby_id = connectionProps.lobby_id, user_id = userId });

        public void SendUpdateSoftBalance(int balance) =>
            wsClient.Send("update_soft_balance", new UpdateSoftBalancePayload(connectionProps.user_id, balance));

        public void SendEmoji(long toUserId, int emojiId) =>
            wsClient.Send("emoji", new EmojiPayload
            {
                lobby_id = connectionProps.lobby_id,
                from_user_id = connectionProps.user_id,
                to_user_id = toUserId,
                emoji_id = emojiId
            });

    }
}
