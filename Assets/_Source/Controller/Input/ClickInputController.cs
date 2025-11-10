using System;
using _Source.Contracts.CustomUpdate;
using _Source.Contracts.DTO.Card;
using _Source.Contracts.DTO.Web;
using MessagePipe;
using Zenject;
using UnityEngine;

namespace _Source.Controller.Input
{
    public class ClickInputController : IDisposable, IInitializable
    {
        private CustomUpdate _customUpdate;

        [Inject] private readonly IPublisher<SelectCardDTO> _selectCardPublisher;
        
        [Inject] private readonly ISubscriber<PlayerConnectedDTO> _playerConnectedSubscriber;
        [Inject] private readonly ISubscriber<SelectInputCardDTO> _selectViewCardSubscriber;

        private bool _isClickedPush;
        private SelectInputCardDTO _lastSelected;
        private bool _isPushed;
        private string _userId;
        
        private DisposableBagBuilder _disposable;
        
        [Inject]
        private void Construct(CustomUpdate customUpdate)
        {
            _customUpdate = customUpdate;
        }
        
        public void Initialize()
        {
            _customUpdate.OnUpdate += Update;

            _disposable = DisposableBag.CreateBuilder();

            _selectViewCardSubscriber.Subscribe((message) => SelectCard(message)).AddTo(_disposable);
            _playerConnectedSubscriber.Subscribe((message) => _userId = message.UserId).AddTo(_disposable);
        }

        private void Update()
        {
            if (UnityEngine.Input.GetMouseButton(0) || UnityEngine.Input.GetMouseButtonDown(0))
                TryPush();
        }

        private void TryPush()
        {
            if (_lastSelected.InCollider && !_isPushed)
            {
                _isPushed = true;
                _selectCardPublisher.Publish(new SelectCardDTO(_lastSelected.CardData, !_lastSelected.Select, _userId));
            }
        }

        private void SelectCard(SelectInputCardDTO message)
        {
            _lastSelected = message;
            _isPushed = false;
        }

        public void Dispose()
        {
            _customUpdate.OnUpdate -= Update;
            
            _disposable.Build().Dispose();
        }
    }
}