using System;
using System.Collections;
using UnityEngine;
using Web.New.Connection.Client;
using Web.New.Connection.Dispatcher;
using Web.New.Connection.Envelope;
using Web.New.Messages.Payloads;
using Web.New.Messages.Payloads.ServerClient;

namespace Web.New.Connection.Router
{
    public class GameEventRouter : MonoBehaviour
    {
        [SerializeField] private MessageDispatcher dispatcher;

        // События для подписки других компонентов игры
        public event Action<ConnectedPayload> OnConnectedToLobby;
        public event Action<PlayerJoinedPayload> OnPlayerJoined;
        public event Action<GamePreparePayload> OnGamePrepare;
        public event Action<GameStartPayload> OnGameStart;
        public event Action<DealPayload> OnDealCards;
        public event Action<ChangeConnectionPayload> OnPlayerConnectionChange;
        public event Action<GameStatePayload> OnGameState;
        public event Action<PlayerStatsPayload> OnPlayerStats;
        public event Action<AuctionTurnRequestPayload> OnAuctionTurnRequest;
        public event Action<PlayRequestPayload> OnPlayRequest;
        public event Action<BetUpdatePayload> OnBetUpdate;
        public event Action<PlayUpdateLLSPayload> OnPlayUpdateLLS;
        public event Action<PlayUpdatePayload> OnPlayUpdate;
        public event Action<PassUpdatePayload> OnPassUpdate;
        public event Action<TurnTimeoutPayload> OnTurnTimeout;
        public event Action<KickPayload> OnPlayerKicked;
        public event Action<GameOverPayload> OnGameOver;
        public event Action<ServerEmojiPayload> OnEmojiReceived;
        public event Action<SpringPayload> OnSpring;
        public event Action<RobberyPayload> OnRobbery;
        public event Action<ErrorPayload> OnErrorReceived;
        public event Action OnConnectionLost;

        [SerializeField] private ConnectionManager connectionManager; // Ссылка на ConnectionManager

        private void Awake()
        {
            if (dispatcher == null)
            {
                Debug.LogError("GameEventRouter: MessageDispatcher не назначен!");
                return;
            }
            if (connectionManager == null)
            {
                Debug.LogError("GameEventRouter: ConnectionManager не назначен!");
                return;
            }

            // Подписка на событие потери соединения от ConnectionManager
            connectionManager.OnConnectionLost += OnConnectionLost;

            // Регистрируем все обработчики событий от сервера
            dispatcher.RegisterHandler("connected", HandleConnected);
            dispatcher.RegisterHandler("player_joined", HandlePlayerJoined);
            dispatcher.RegisterHandler("game_prepare", HandleGamePrepare);
            dispatcher.RegisterHandler("change_connection", HandleChangeConnection);
            dispatcher.RegisterHandler("game_start", HandleGameStart);
            dispatcher.RegisterHandler("deal", HandleDeal);
            dispatcher.RegisterHandler("auction_turn_request", HandleAuctionTurnRequest);
            dispatcher.RegisterHandler("game_state", HandleGameState);
            dispatcher.RegisterHandler("player_stats", HandlePlayerStats);
            dispatcher.RegisterHandler("play_request", HandlePlayRequest);
            dispatcher.RegisterHandler("bet_update", HandleBetUpdate);
            dispatcher.RegisterHandler("play_update_lls", HandlePlayUpdateLLS);
            dispatcher.RegisterHandler("play_update", HandlePlayUpdate);
            dispatcher.RegisterHandler("pass_update", HandlePassUpdate);
            dispatcher.RegisterHandler("turn_timeout", HandleTurnTimeout);
            dispatcher.RegisterHandler("kick", HandleKick);
            dispatcher.RegisterHandler("game_over", HandleGameOver);
            dispatcher.RegisterHandler("emoji", HandleEmoji);
            dispatcher.RegisterHandler("spring", HandleSpring);
            dispatcher.RegisterHandler("robbery", HandleRobbery);
            dispatcher.RegisterHandler("error", HandleError);
            // "ping" и "pong" обрабатываются в ConnectionManager и не пробрасываются сюда.
        }

        public IEnumerator TEST()
        {
            yield return new WaitForSeconds(10);
            string a = "{\"event\":\"game_state\",\"data\":{\"lobby_id\":7,\"players\":[{\"user_id\":7,\"name\":\"kristoferrabbit\",\"cards\":[0,10,14,17,20,22,24,27,31,32,35,40,42,47,48,50,53],\"missed_turns\":0,\"last_move\":[],\"balance\":575,\"is_connected\":true,\"bet\":0,\"avatar_url\":\"\"},{\"user_id\":10,\"name\":\"Evrobelbiz\",\"cards\":[2,4,7,9,11,15,18,19,23,28,33,34,36,44,49,51,52],\"missed_turns\":0,\"last_move\":[],\"balance\":820,\"is_connected\":true,\"bet\":0,\"avatar_url\":\"\"},{\"user_id\":5,\"name\":\"shumanmax\",\"cards\":[1,3,5,6,12,13,16,25,26,29,30,37,39,41,43,45,46],\"missed_turns\":0,\"last_move\":[],\"balance\":4700,\"is_connected\":true,\"bet\":0,\"avatar_url\":\"\"}],\"phase\":\"dealing\",\"landlord\":{\"selected\":false,\"user_id\":0},\"start_bet\":50,\"max_bet\":250,\"multiplier\":1,\"kitty_cards\":[],\"current_player_id\":0}}";
            dispatcher.Dispatch(a);
        }

        private void HandleConnected(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<ConnectedPayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                OnConnectedToLobby?.Invoke(envelope.data);
                Debug.Log($"GameEventRouter: Подключен к лобби {envelope.data.lobby_id}. Ставка: {envelope.data.bet}");
            }
        }

        private void HandlePlayerJoined(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<PlayerJoinedPayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                OnPlayerJoined?.Invoke(envelope.data);
                Debug.Log($"GameEventRouter: Игрок {envelope.data.name} ({envelope.data.user_id}) присоединился.");
            }
        }

        private void HandleChangeConnection(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<ChangeConnectionPayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                OnPlayerConnectionChange?.Invoke(envelope.data);
                Debug.Log($"GameEventRouter: Игрок {envelope.data.user_id} изменил статус соединения на: {envelope.data.is_connected}.");
            }
        }

        private void HandleGamePrepare(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<GamePreparePayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                OnGamePrepare?.Invoke(envelope.data);
                Debug.Log($"GameEventRouter: Все игроки вошли. Переход к загрузке сцены. Время на готовность: {envelope.data.timeout}");
            }
        }

        private void HandleGameStart(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<GameStartPayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                Debug.Log($"GameEventRouter: Игра началась! Землевладелец: {envelope.data.landlord_id}. Начат вызов событий.");
                OnGameStart?.Invoke(envelope.data);
                Debug.Log($"GameEventRouter: События вызваны!");
            }
        }

        private void HandlePlayerStats(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<PlayerStatsPayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                Debug.Log($"GameEventRouter: Получены данные игрока {envelope.data.user_id}");
                OnPlayerStats?.Invoke(envelope.data);
                Debug.Log($"GameEventRouter: События вызваны!");
            }
        }

        private void HandleDeal(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<DealPayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                OnDealCards?.Invoke(envelope.data);
                Debug.Log($"GameEventRouter: Получена раздача карт. Количество: {envelope.data.cards.Length}");
            }
        }

        private void HandleBetUpdate(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<BetUpdatePayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                OnBetUpdate?.Invoke(envelope.data);
                Debug.Log($"GameEventRouter: Получено обновление ставки для игрока {envelope.data.user_id}, текущая ставка : {envelope.data.bet}");
            }
        }

        private void HandlePlayUpdateLLS(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<PlayUpdateLLSPayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                OnPlayUpdateLLS?.Invoke(envelope.data);
                Debug.Log($"GameEventRouter: Игрок {envelope.data.user_id} сделал ход, текущая ставка : {envelope.data.current_bid}");
            }
        }

        private void HandleAuctionTurnRequest(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<AuctionTurnRequestPayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                OnAuctionTurnRequest?.Invoke(envelope.data);
                Debug.Log($"GameEventRouter: Запрос хода к игроку (аукцион): {envelope.data.current_player}");
            }
        }

        private void HandlePlayRequest(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<PlayRequestPayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                OnPlayRequest?.Invoke(envelope.data);
                Debug.Log($"GameEventRouter: Запрос хода к игроку: {envelope.data.current_player}");
            }
        }

        private void HandleGameState(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<GameStatePayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                OnGameState?.Invoke(envelope.data);
                Debug.Log($"GameEventRouter: Получен GameState");
            }
        }

        private void HandlePlayUpdate(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<PlayUpdatePayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                OnPlayUpdate?.Invoke(envelope.data);
                Debug.Log($"GameEventRouter: Игрок {envelope.data.user_id} сделал ход с картами: {string.Join(", ", envelope.data.cards)}");
            }
        }

        private void HandlePassUpdate(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<PassUpdatePayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                OnPassUpdate?.Invoke(envelope.data);
                Debug.Log($"GameEventRouter: Игрок {envelope.data.user_id} пропустил ход.");
            }
        }

        private void HandleTurnTimeout(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<TurnTimeoutPayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                OnTurnTimeout?.Invoke(envelope.data);
                Debug.Log($"GameEventRouter: Ход игрока {envelope.data.user_id} отменен по таймауту.");
            }
        }

        private void HandleKick(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<KickPayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                OnPlayerKicked?.Invoke(envelope.data);
                Debug.LogWarning($"GameEventRouter: Игрок {envelope.data.user_id} кикнут. Причина: {envelope.data.reason}");
                // здесьы можно вызвать DisconnectGameConnection, если кик означает полное отключение
                // connectionManager.DisconnectGameConnection();
            }
        }

        private void HandleGameOver(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<GameOverPayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                OnGameOver?.Invoke(envelope.data);
                Debug.Log($"GameEventRouter: Игра окончена! Победитель: {envelope.data.winner_id}");
            }
        }

        private void HandleEmoji(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<ServerEmojiPayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                OnEmojiReceived?.Invoke(envelope.data);
                Debug.Log($"GameEventRouter: Эмодзи от {envelope.data.from_user_id} к {envelope.data.to_user_id}. Emoji ID: {envelope.data.emoji_id}");
            }
        }

        private void HandleSpring(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<SpringPayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                OnSpring?.Invoke(envelope.data);
            }
        }

        private void HandleRobbery(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<RobberyPayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                OnRobbery?.Invoke(envelope.data);
            }
        }

        private void HandleError(string rawMessage)
        {
            var envelope = JsonUtility.FromJson<WebSocketEnvelope<ErrorPayload>>(rawMessage);
            if (envelope != null && envelope.data != null)
            {
                OnErrorReceived?.Invoke(envelope.data);
                Debug.LogError($"GameEventRouter: Ошибка от сервера! Код: {envelope.data.code}, Сообщение: {envelope.data.message}");
            }
        }

        private void OnDestroy()
        {
            // Отписываемся от событий, чтобы избежать утечек памяти
            if (connectionManager != null)
            {
                connectionManager.OnConnectionLost -= OnConnectionLost;
            }
            if (dispatcher != null)
            {
                dispatcher.UnregisterHandler("connected", HandleConnected);
                dispatcher.UnregisterHandler("player_joined", HandlePlayerJoined);
                dispatcher.UnregisterHandler("game_start", HandleGameStart);
                dispatcher.UnregisterHandler("deal", HandleDeal);
                dispatcher.UnregisterHandler("play_request", HandlePlayRequest);
                dispatcher.UnregisterHandler("play_update", HandlePlayUpdate);
                dispatcher.UnregisterHandler("pass_update", HandlePassUpdate);
                dispatcher.UnregisterHandler("turn_timeout", HandleTurnTimeout);
                dispatcher.UnregisterHandler("kick", HandleKick);
                dispatcher.UnregisterHandler("game_over", HandleGameOver);
                dispatcher.UnregisterHandler("emoji", HandleEmoji);
                dispatcher.UnregisterHandler("error", HandleError);
            }
        }
    }
}