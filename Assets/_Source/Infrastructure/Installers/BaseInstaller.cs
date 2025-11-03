using _Source.Application.GameState;
using _Source.Application.Lobby;
using _Source.Application.Players;
using _Source.Application.Web;
using _Source.Contracts.CustomUpdate;
using _Source.Controller.Input;
using _Source.Domain.Card;
using _Source.Domain.GameLobby;
using _Source.Infrastructure.Factory;
using UnityEngine;
using Zenject;

public class BaseInstaller : MonoInstaller
{
    [SerializeField] private int _updateDelay;
    
    public override void InstallBindings()
    {
        DomainInstaller();
        
        WebInstall();

        ControllersInstaller();
        
        FactoryInstall();

        GameServiceInstaller();
    }

    private void DomainInstaller()
    {
        CustomUpdate customUpdate = new CustomUpdate(_updateDelay);
        Container.Bind<CustomUpdate>().FromInstance(customUpdate).AsSingle();
        
        Container.BindInterfacesAndSelfTo<CardGridModel>().AsSingle();
    }

    private void WebInstall()
    {
        Container.BindInterfacesAndSelfTo<WebSocketConnectionService>().AsSingle();
        Container.Bind<WebReceiverService>().AsSingle();
        Container.Bind<WebMessageService>().AsSingle();
        Container.Bind<GameLobbyModel>().AsSingle();
    }

    private void ControllersInstaller()
    {
        Container.BindInterfacesAndSelfTo<ClickInputController>().AsSingle();
    }

    private void FactoryInstall()
    {
        Container.BindInterfacesAndSelfTo<PlayerFactory>().AsSingle();
        Container.BindInterfacesAndSelfTo<CardFactory>().AsSingle();
    }

    private void GameServiceInstaller()
    {
        Container.BindInterfacesAndSelfTo<GameStateService>().AsSingle();
        Container.BindInterfacesAndSelfTo<SetupPlayerInfoService>().AsSingle();
        Container.BindInterfacesAndSelfTo<LobbyInfoService>().AsSingle();
    }
}