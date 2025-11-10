using System;
using System.Linq;
using _Source.Contracts.DataBase;
using _Source.Contracts.View;
using _Source.Presentation.StartPositions;
using _Source.Presentation.View.PlayerView;
using UnityEngine;

namespace _Source.Infrastructure.Repositories.Player
{
    [CreateAssetMenu(fileName = "PlayerDataBase", menuName = "ScriptableObjects/DataBase/Player/PlayerDataBase", order = 1)]
    public class PlayerDataBase : ScriptableObject, IPlayerDataBase
    {
        [SerializeField] private PlayerView _playerViewPrefab;
        [SerializeField] private PlayerViewSkinData[] _allSkins;

        public IPlayerView PlayerViewPrefab => _playerViewPrefab;
        
        public  Sprite[] GetSkin(int number)
        {
            return _allSkins.First(x => x.Number == number).Sprites;
        }
    }
    
        
    [Serializable]
    public struct PlayerViewSkinData
    {
        public int Number;
        public Sprite[] Sprites;
    }
}