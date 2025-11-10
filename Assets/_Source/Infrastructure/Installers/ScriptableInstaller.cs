using _Source.Contracts.DataBase;
using _Source.Infrastructure.Repositories.CardDatabase;
using _Source.Infrastructure.Repositories.Player;
using _Source.Infrastructure.Repositories.Web;
using UnityEngine;
using Zenject;

namespace _Source.Infrastructure.Installers
{
    [CreateAssetMenu(fileName = "ScriptableInstaller", menuName = "Installers/ScriptableInstaller")]
    public class ScriptableInstaller : ScriptableObjectInstaller<ScriptableInstaller>
    {
        [SerializeField] private WebConfig _webConfig;
        [SerializeField] private CardDatabase _cardDatabase;
        [SerializeField] private PlayerDataBase _playerDataBase;

        public override void InstallBindings()
        {
            Container.Bind<IWebConfig>().FromInstance(_webConfig);
            Container.Bind<ICardDataBase>().FromInstance(_cardDatabase);
            Container.Bind<IPlayerDataBase>().FromInstance(_playerDataBase);
        }
    }
}