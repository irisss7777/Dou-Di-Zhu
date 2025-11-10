using TMPro;
using UnityEngine;

namespace _Source.Contracts.View
{
    public interface IPlayerView
    {
        public void SetupCharacterSprite(Sprite[] newSprites, int direction, TMP_Text passText);

        public void SetupTextInfo(string name, Vector2 position);

        public void Leave(string name);
    }
}