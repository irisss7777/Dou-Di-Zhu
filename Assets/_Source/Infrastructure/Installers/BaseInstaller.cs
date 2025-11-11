using _Source.Application.Cards;
using _Source.Application.GameState;
using _Source.Application.Lobby;
using _Source.Application.Players;
using _Source.Application.Web;
using _Source.Contracts.CustomUpdate;
using _Source.Contracts.Factory;
using _Source.Contracts.Player;
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
        MessageBus();

        BaseService();
        
        DomainInstaller();
        
        WebInstall();

        ControllersInstaller();
        
        FactoryInstall();

        GameServiceInstaller();
    }

    private void MessageBus()
    {
        Container.BindInterfacesAndSelfTo<PlayerMessageBusService>().AsSingle();
    }

    private void BaseService()
    {
        Container.BindInterfacesAndSelfTo<PlayerCardsService>().AsSingle();
    }

    private void DomainInstaller()
    {
        Container.Bind<CustomUpdate>().FromInstance(new CustomUpdate(_updateDelay)).AsSingle();
        
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
        Container.BindInterfacesAndSelfTo<ClickInputController>().AsSingle().NonLazy();
    }

    private void FactoryInstall()
    {
        Container.BindInterfacesAndSelfTo<PlayerFactory>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<CardFactory>().AsSingle().NonLazy();
    }

    private void GameServiceInstaller()
    {
        Container.BindInterfacesAndSelfTo<GameStateService>().AsSingle();
        Container.BindInterfacesAndSelfTo<SetupPlayerInfoService>().AsSingle();
        Container.BindInterfacesAndSelfTo<LobbyInfoService>().AsSingle();
        Container.BindInterfacesAndSelfTo<MoveCardsService>().AsSingle();
    }
}