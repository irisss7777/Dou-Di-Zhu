using TMPro;
using UnityEngine;

namespace _Source.Presentation.View.PlayerInfoView
{
    public class CurrentPlayerInfoView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _userNameText;
        [SerializeField] private TMP_Text _userIdText;
        
        public TMP_Text UserNameText => _userNameText;
        public TMP_Text UserIdText => _userIdText;
    }
}