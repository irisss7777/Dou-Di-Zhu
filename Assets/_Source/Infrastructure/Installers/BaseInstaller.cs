using _Source.Application.GameState;
using _Source.Application.Lobby;
using _Source.Application.Players;
using _Source.Application.Web;
using _Source.Domain.GameLobby;
using _Source.Infrastructure.Factory;
using Zenject;

public class BaseInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        WebInstall();

        FactoryInstall();

        GameServiceInstaller();
    }

    private void WebInstall()
    {
        Container.BindInterfacesAndSelfTo<WebSocketConnectionService>().AsSingle();
        Container.Bind<WebReceiverService>().AsSingle();
        Container.Bind<WebMessageService>().AsSingle();
        Container.Bind<GameLobbyModel>().AsSingle();
    }

    private void FactoryInstall()
    {
        Container.BindInterfacesAndSelfTo<PlayerFactory>().AsSingle();
    }

    private void GameServiceInstaller()
    {
        Container.BindInterfacesAndSelfTo<GameStateService>().AsSingle();
        Container.BindInterfacesAndSelfTo<SetupPlayerInfoService>().AsSingle();
        Container.BindInterfacesAndSelfTo<LobbyInfoService>().AsSingle();
    }
}