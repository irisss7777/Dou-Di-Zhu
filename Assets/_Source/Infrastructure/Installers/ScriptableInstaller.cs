using _Source.Contracts.DataBase;
using _Source.Infrastructure.Repositories.Web;
using UnityEngine;
using Zenject;

namespace _Source.Infrastructure.Installers
{
    [CreateAssetMenu(fileName = "ScriptableInstaller", menuName = "Installers/ScriptableInstaller")]
    public class ScriptableInstaller : ScriptableObjectInstaller<ScriptableInstaller>
    {
        [SerializeField] private WebConfig _webConfig;

        public override void InstallBindings()
        {
            Container.Bind<IWebConfig>().FromInstance(_webConfig);
        }
    }
}