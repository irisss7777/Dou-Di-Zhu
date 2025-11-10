using UnityEngine;

namespace _Source.Presentation.View.PlayerView
{
    public class CharacterAnimation : MonoBehaviour
    {
        [SerializeField] private float breathAmount = 0.05f;
        [SerializeField] private float breathSpeed = 2f;
        [SerializeField] private SpriteRenderer playerSprite;

        public Sprite[] Sprites
        {
            get { return sprites; }
            set
            {
                sprites = value;
                playerSprite.sprite = value[0];
            }
        }

        [SerializeField] private Sprite[] sprites;

        [SerializeField] private bool fixPositionX = true;
        [SerializeField] private bool fixPositionY = true;

        private Vector3 initialScale;
        private Vector3 initialPosition;

        private float blinkDelay;
        private float blinkDuration = 0.3f;
        private float currentBlinkDuration;
        private bool isBlink = false;

        void Start()
        {
            initialScale = transform.localScale;
            initialPosition = transform.position;

            blinkDelay = Random.Range(3.00f, 10.00f);
        }
        
        public void SetupSprite(Sprite[] newSprites)
        {
            sprites = newSprites;
            playerSprite.sprite = newSprites[0];
        }

        void Update()
        {
            float scaleY = initialScale.y + Mathf.Sin(Time.unscaledTime * breathSpeed) * breathAmount;
            float deltaY = (scaleY - initialScale.y) * -3f;

            transform.localScale = new Vector3(initialScale.x, scaleY, initialScale.z);
            transform.position = new Vector3(fixPositionX ? initialPosition.x : transform.position.x,
                (fixPositionY ? initialPosition.y : transform.position.y) - deltaY, initialPosition.z);
            if (!isBlink)
            {
                blinkDelay -= Time.unscaledDeltaTime;
                if (blinkDelay <= 0f)
                {
                    Blink();
                }
            }
            else
            {
                currentBlinkDuration -= Time.unscaledDeltaTime;
                if (currentBlinkDuration <= 0f)
                {
                    Unblink();
                }
            }

        }

        private void Blink()
        {
            if (sprites.Length >= 1)
            {
                playerSprite.sprite = sprites[1];
                currentBlinkDuration = blinkDuration;
                isBlink = true;
            }
        }

        private void Unblink()
        {
            if (sprites.Length >= 1)
            {
                playerSprite.sprite = sprites[0];
                blinkDelay = Random.Range(3.00f, 10.00f);
                isBlink = false;
            }
        }
    }
}