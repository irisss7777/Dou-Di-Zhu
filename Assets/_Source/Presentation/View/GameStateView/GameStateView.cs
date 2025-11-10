using UnityEngine;
using TMPro;

namespace _Source.Presentation.View.GameStateView
{
    public class GameStateView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _playButtonsGroup;
        
        [SerializeField] private CanvasGroup _waitLobbyGroup;
        
        [SerializeField] private CanvasGroup _startGameGroup;
        
        [SerializeField] private CanvasGroup _connectionErrorGroup;
        [SerializeField] private TMP_Text _connectionErrorText;
        
        public CanvasGroup PlayButtonsGroup => _playButtonsGroup;
        
        public CanvasGroup WaitLobbyGroup => _waitLobbyGroup;
        
        public CanvasGroup StartGameGroup => _startGameGroup;
        
        public CanvasGroup ConnectionErrorGroup => _connectionErrorGroup;
        public TMP_Text ConnectionErrorText => _connectionErrorText;
    }
}