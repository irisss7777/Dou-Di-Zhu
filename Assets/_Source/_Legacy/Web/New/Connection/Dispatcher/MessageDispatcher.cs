using System;
using System.Collections.Generic;
using UnityEngine;
using Web.New.Connection.Client;
using Web.New.Connection.Envelope;
using Web.New.Messages.Payloads;

namespace Web.New.Connection.Dispatcher
{
    public class MessageDispatcher : MonoBehaviour
    {
        private readonly Dictionary<string, Action<string>> handlers = new Dictionary<string, Action<string>>();

        [SerializeField] private WebSocketClient wsClient;

        void Awake()
        {
            if (wsClient == null)
            {
                Debug.LogError("MessageDispatcher: WebSocketClient не назначен!");
                return;
            }
            wsClient.OnRawMessageReceived += Dispatch;
        }

        public void RegisterHandler(string eventName, Action<string> handler)
        {
            if (handlers.ContainsKey(eventName))
                handlers[eventName] += handler;
            else
                handlers[eventName] = handler;
            Debug.Log($"MessageDispatcher: Зарегистрирован обработчик для события '{eventName}'");
        }

        public void UnregisterHandler(string eventName, Action<string> handler)
        {
            if (!handlers.ContainsKey(eventName)) return;
            handlers[eventName] -= handler;
            if (handlers[eventName] == null)
                handlers.Remove(eventName);
            Debug.Log($"MessageDispatcher: Отменен обработчик для события '{eventName}'");
        }

        public void Dispatch(string rawMessage)
        {
            WebSocketEnvelope<object> tempEnvelope = null;
            //try
            //{
                // Попытка десериализации в базовый Envelope для получения имени события
                tempEnvelope = JsonUtility.FromJson<WebSocketEnvelope<object>>(rawMessage);

                if (tempEnvelope == null)
                {
                    Debug.LogError($"MessageDispatcher: Не удалось десериализовать базовый Envelope. Вероятно, невалидный JSON: {rawMessage}");
                    // Можно отправить ошибку на UI или в лог системы мониторинга
                    HandleDeserializationError("invalid_json_format", "Не удалось десериализовать базовый Envelope: невалидный JSON", rawMessage);
                    return;
                }

                if (string.IsNullOrEmpty(tempEnvelope.@event))
                {
                    Debug.LogWarning("MessageDispatcher: Невалидный формат сообщения: отсутствует 'event' поле.");
                    HandleDeserializationError("missing_event_field", "Отсутствует поле 'event' в сообщении", rawMessage);
                    return;
                }

                if (handlers.TryGetValue(tempEnvelope.@event, out var handler))
                {
                    // Передаем весь сырой JSON обработчику, чтобы он сам десериализовал свой Payload
                    handler.Invoke(rawMessage);
                }
                else
                {
                    Debug.LogWarning($"MessageDispatcher: Нет обработчика для события: '{tempEnvelope.@event}'");
                    HandleDeserializationError("unhandled_event", $"Нет зарегистрированного обработчика для события: '{tempEnvelope.@event}'", rawMessage);
                }
            //}
            //catch (Exception ex)
            //{
            //    Debug.LogError($"MessageDispatcher: Общая ошибка при Dispatch: {ex.Message}\nСообщение: {rawMessage}\nСтек: {ex.StackTrace}");
            //    HandleDeserializationError("dispatch_exception", $"Исключение при диспетчеризации: {ex.Message}", rawMessage);
            //}
        }
        
        // Унифицированный метод для обработки и логирования ошибок десериализации/диспетчеризации.
        // Можно использовать для отправки ошибки в ErrorRouter или на UI.
        private void HandleDeserializationError(string errorCode, string errorMessage, string rawMessage)
        {
            // Здесь можно вызвать событие, которое GameEventRouter (или другой ErrorRouter) подпишет и обработает.
            // Например, можно вызвать:
            // OnDispatchError?.Invoke(new ErrorPayload { code = SOME_ERROR_CODE, message = errorMessage, rawMessage = rawMessage });
            // Или просто логировать и не пробрасывать дальше, если это "внутренняя" ошибка диспетчера.

            // Пока оставим просто логирование, как самое безопасное:
            Debug.LogError($"[ERROR_DISPATCHER] Code: {errorCode}, Message: {errorMessage}, Raw: {rawMessage}");
        }

        void OnDestroy()
        {
            if (wsClient != null)
            {
                wsClient.OnRawMessageReceived -= Dispatch;
            }
        }
    }
}