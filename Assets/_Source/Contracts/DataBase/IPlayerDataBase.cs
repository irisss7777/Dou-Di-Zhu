using _Source.Contracts.View;
using UnityEngine;

namespace _Source.Contracts.DataBase
{
    public interface IPlayerDataBase
    {
        public IPlayerView PlayerViewPrefab { get; }

        public Sprite[] GetSkin(int number);
    }
}