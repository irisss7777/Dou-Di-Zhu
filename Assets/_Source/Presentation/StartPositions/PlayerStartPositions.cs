using System;
using System.Collections.Generic;
using System.Linq;
using _Source.Contracts.DTO.Web;
using MessagePipe;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Source.Presentation.StartPositions
{
    public class PlayerStartPositions : MonoBehaviour
    {
        [SerializeField] private PlayerViewPositionData[] _otherPosition;
        [SerializeField] private PlayerViewPositionData _myPlayerPosition;
        
        private int _currentPosition;

        public PlayerViewPositionData GetPosition()
        {
            PlayerViewPositionData target = _otherPosition[_currentPosition];
            _currentPosition++;
            return target;
        }

        public PlayerViewPositionData GetMyPosition()
        {
            return _myPlayerPosition;
        }

        public PlayerViewPositionData GetData(string name)
        {
            List<PlayerViewPositionData> playerDatas = new();
            
            playerDatas.Add(_otherPosition[0]);
            playerDatas.Add(_otherPosition[1]);
            playerDatas.Add(_myPlayerPosition);

            return playerDatas.First(x => x.Name == name);
        }

        public void SetDataName(string name, Transform target)
        {
            if (_otherPosition[0].Transform == target)
            {
                _otherPosition[0].Name = name;
            }
            
            if (_otherPosition[1].Transform == target)
            {
                _otherPosition[1].Name = name;
            }
            
            if (_myPlayerPosition.Transform == target)
            {
                _myPlayerPosition.Name = name;
            }
        }
    }

    [Serializable]
    public struct PlayerViewPositionData
    {
        public string Name;
        public Transform Transform;
        public Transform UsedCardTransform;
        public int Direction;
        public Vector2 InfoPanelPosition;
        public TMP_Text PassText;

        public void SetName(string name)
        {
            Name = name;
        }
    }
}