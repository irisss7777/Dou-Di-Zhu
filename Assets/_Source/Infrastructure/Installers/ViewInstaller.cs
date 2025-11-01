using _Source.Presentation.View.GameStateView;
using _Source.Presentation.View.LobbyInfoView;
using _Source.Presentation.View.PlayerInfoView;
using UnityEngine;
using Zenject;

namespace _Source.Infrastructure.Installers
{
    public class ViewInstaller : MonoInstaller
    {
        [SerializeField] private GameStateView _gameStateView;
        [SerializeField] private CurrentPlayerInfoView _currentPlayerInfoView;
        [SerializeField] private LobbyInfoView _lobbyInfoView;
        
        public override void InstallBindings()
        {
            Container.Bind<GameStateView>().FromInstance(_gameStateView).AsSingle();
            Container.Bind<CurrentPlayerInfoView>().FromInstance(_currentPlayerInfoView).AsSingle();
            Container.Bind<LobbyInfoView>().FromInstance(_lobbyInfoView).AsSingle();
        }
    }
}