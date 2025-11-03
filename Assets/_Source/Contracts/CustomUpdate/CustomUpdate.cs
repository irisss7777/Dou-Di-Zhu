using System;
using Cysharp.Threading.Tasks;

namespace _Source.Contracts.CustomUpdate
{
    public class CustomUpdate
    {
        private int _delay;

        public Action OnUpdate;

        public CustomUpdate(int updateDelay)
        {
            _delay = updateDelay;
            
            Update().Forget();
        }

        private async UniTask Update()
        {
            while (true)
            {
                OnUpdate?.Invoke();

                await UniTask.Delay(_delay);
            }
        }
    }
}