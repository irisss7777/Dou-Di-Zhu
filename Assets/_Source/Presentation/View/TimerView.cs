using System;
using _Source.Contracts.DTO.Web;
using Cysharp.Threading.Tasks;
using MessagePipe;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Source.Presentation.View
{
    public class TimerView : MonoBehaviour
    {
        [Inject] private readonly ISubscriber<SetMoveStateDTO> _playerConnectedSubscriber;
        
        private DisposableBagBuilder _disposable;
        
        [SerializeField] private TMP_Text _timerText;

        private void Start()
        {
            _disposable = DisposableBag.CreateBuilder();

            _playerConnectedSubscriber.Subscribe((message) => StartTimer(message.Time, message.MaxTime)).AddTo(_disposable);
        }

        private void StartTimer(float time, float maxTime)
        {
            _timerText.text = "" + ((maxTime - time) + 1);
        }

        private void OnDestroy()
        {
            _disposable.Build().Dispose();
        }
    }
}