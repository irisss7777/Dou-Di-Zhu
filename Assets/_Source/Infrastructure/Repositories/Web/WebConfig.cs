using _Source.Contracts.DataBase;
using UnityEngine;

namespace _Source.Infrastructure.Repositories.Web
{
    [CreateAssetMenu(fileName = "WebConfig", menuName = "ScriptableObjects/DataBase/Web/WebConfig", order = 1)]
    public class WebConfig : ScriptableObject, IWebConfig
    {
        [SerializeField] private string _serverBaseUrl = "wss://tma-game.ru/DouDiZhu/ws/";

        public string ServerBaseUrl => _serverBaseUrl;
    }
}