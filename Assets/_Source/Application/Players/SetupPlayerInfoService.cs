using System;
using _Source.Contracts.DTO.Player;
using _Source.Presentation.View.PlayerInfoView;
using MessagePipe;
using Zenject;

namespace _Source.Application.Players
{
    public class SetupPlayerInfoService : IDisposable, IInitializable
    {
        [Inject]  private readonly ISubscriber<CurrentPlayerAddedDTO> _playerAddedPublisher;

        private CurrentPlayerInfoView _currentPlayerInfoView;
        
        private DisposableBagBuilder _disposable;

        [Inject]
        private void Construct(CurrentPlayerInfoView currentPlayerInfoView)
        {
            _currentPlayerInfoView = currentPlayerInfoView;
        }
        
        public void Initialize()
        {
            _disposable = DisposableBag.CreateBuilder();
            
            _playerAddedPublisher.Subscribe((message) => OnCurrentPlayerConnected(message.Name, message.UserId)).AddTo(_disposable);
        }

        private void OnCurrentPlayerConnected(string name, string userId)
        {
            if (_currentPlayerInfoView.UserIdText.text == "")
            {
                _currentPlayerInfoView.UserNameText.text = name;
                _currentPlayerInfoView.UserIdText.text = userId;
            }
        }

        public void Dispose()
        {

        }
    }
}