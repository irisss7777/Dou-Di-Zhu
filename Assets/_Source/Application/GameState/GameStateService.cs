using System;
using _Source.Contracts.DTO.GameStage;
using _Source.Contracts.DTO.Web;
using _Source.Contracts.DTO.Web.WebsocketConnectionDTO;
using _Source.Presentation.View.GameStateView;
using MessagePipe;
using Zenject;
using DG.Tweening;
using UnityEngine;

namespace _Source.Application.GameState
{
    public class GameStateService : IDisposable, IInitializable
    {
        [Inject] private readonly ISubscriber<SetMoveStateDTO> _playerConnectedSubscriber;
        [Inject] private readonly ISubscriber<GameActiveStateDTO> _gameActiveStateSubscriber;
        [Inject] private readonly ISubscriber<LobbyInfoStateDTO> _lobbyInfoStateSubscriber;
        [Inject] private readonly ISubscriber<ErrorDTO> _errorConnectionSubscriber;
        [Inject] private readonly ISubscriber<ConnectionDTO> _connectionSubscriber;

        private GameStateView _gameStateView;
        
        private DisposableBagBuilder _disposable;

        [Inject]
        private void Construct(GameStateView gameStateView)
        {
            _gameStateView = gameStateView;
        }
        
        public void Initialize()
        {
            _disposable = DisposableBag.CreateBuilder();
            
            _gameActiveStateSubscriber.Subscribe((message) => SetGameActive(message.GameActive)).AddTo(_disposable);
            _errorConnectionSubscriber.Subscribe((message) => SetConnectionErrorActive(message.ErrorMessage, true)).AddTo(_disposable);
            _connectionSubscriber.Subscribe((message) => SetConnectionErrorActive("Connection error", !message.Connection)).AddTo(_disposable);
            _lobbyInfoStateSubscriber.Subscribe((message) => SetLobbyInfoActive(message.LobbyActive)).AddTo(_disposable);
            _playerConnectedSubscriber.Subscribe((message) => SetPlayButtonsActive(message.MoveState)).AddTo(_disposable);
        }

        private void SetConnectionErrorActive(string errorMessage, bool active)
        {
            _gameStateView.ConnectionErrorText.text = errorMessage;
                
            var targetAlpha = active ? 1f : 0f;

            if (active)
            {
                _gameStateView.ConnectionErrorGroup.gameObject.SetActive(true);
                _gameStateView.ConnectionErrorGroup.DOFade(targetAlpha, 0.5f);
            }
            else
            {
                _gameStateView.ConnectionErrorGroup.DOFade(targetAlpha, 0.5f).OnComplete(() =>
                    _gameStateView.ConnectionErrorGroup.gameObject.SetActive(false));
            }
        }

        private void SetPlayButtonsActive(bool active)
        {
            var targetAlpha = active ? 1f : 0f;

            if (active)
                _gameStateView.PlayButtonsGroup.gameObject.SetActive(true);
            
            _gameStateView.PlayButtonsGroup.DOFade(targetAlpha, 1f).OnComplete(() =>
                _gameStateView.PlayButtonsGroup.gameObject.SetActive(active));
        }

        private void SetGameActive(bool active)
        {
            SetConnectionErrorActive("", false);
            var targetAlpha = active ? 0f : 1f;
            _gameStateView.StartGameGroup.DOFade(targetAlpha, 1f).OnComplete(() =>
                _gameStateView.StartGameGroup.gameObject.SetActive(false));
        }
        
        private void SetLobbyInfoActive(bool active)
        {
            var targetAlpha = active ? 0f : 1f;
            _gameStateView.WaitLobbyGroup.DOFade(targetAlpha, 1f).OnComplete(() =>
                _gameStateView.WaitLobbyGroup.gameObject.SetActive(false));
        }

        public void Dispose()
        {
            _disposable.Build().Dispose();
        }
    }
}