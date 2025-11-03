using _Source.Presentation.StartPositions;
using _Source.Presentation.View.GameStateView;
using _Source.Presentation.View.LobbyInfoView;
using _Source.Presentation.View.PlayerInfoView;
using UnityEngine;
using Zenject;

namespace _Source.Infrastructure.Installers
{
    public class ViewInstaller : MonoInstaller
    {
        [Header("Views")]
        [SerializeField] private GameStateView _gameStateView;
        [SerializeField] private CurrentPlayerInfoView _currentPlayerInfoView;
        [SerializeField] private LobbyInfoView _lobbyInfoView;
        
        [Header("Start positions")]
        [SerializeField] private CardStartPosition _cardStartPosition;
        
        public override void InstallBindings()
        {
            InstallViews();

            InstallStartPositions();
        }

        private void InstallViews()
        {
            Container.Bind<GameStateView>().FromInstance(_gameStateView).AsSingle();
            Container.Bind<CurrentPlayerInfoView>().FromInstance(_currentPlayerInfoView).AsSingle();
            Container.Bind<LobbyInfoView>().FromInstance(_lobbyInfoView).AsSingle();
        }

        private void InstallStartPositions()
        {
            Container.Bind<CardStartPosition>().FromInstance(_cardStartPosition).AsSingle();
        }
    }
}