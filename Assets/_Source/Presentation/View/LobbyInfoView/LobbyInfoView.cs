using TMPro;
using UnityEngine;

namespace _Source.Presentation.View.LobbyInfoView
{
    public class LobbyInfoView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _playerCountText;
        [SerializeField] private TMP_Text _playerNamesText;

        public TMP_Text PlayerCountText => _playerCountText;
        public TMP_Text PlayerNamesText => _playerNamesText;

    }
}