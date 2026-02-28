using RoboClean.Input;
using RoboClean.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace RoboClean.UI
{
    public class InputDeviceSpriteSwapHelper : DestoryOnlyIfAwakeMonoBehavior
    {
        [SerializeField] private Sprite KEYBOARD_SPRITE;
        [SerializeField] private Sprite GAMEPAD_SPRITE;
        [SerializeField] private Sprite MOBILE_SPRITE;

        private void Start()
        {
            InputManager.Instance.LoadedInputHandler.CurrentInputDevice.AddListener(spriteSwap, true);
        }

        protected override void onDisable()
        {
        }

        protected override void onDestory()
        {
            if (InputManager.IsExist)
            {
                InputManager.Instance.LoadedInputHandler.CurrentInputDevice.RemoveListener(spriteSwap);
            }
        }

        public void spriteSwap(eInputDeviceType isGamePad)
        {
            Sprite convertedSprite = null;
            switch (isGamePad)
            {
                case eInputDeviceType.KeyboardAndMouse:
                    convertedSprite = KEYBOARD_SPRITE;
                    break;
                case eInputDeviceType.GamePad:
                    convertedSprite = GAMEPAD_SPRITE;
                    break;
                case eInputDeviceType.Mobile:
                    convertedSprite = MOBILE_SPRITE;
                    break;
                default:
                    break;
            }

            if (convertedSprite == null)
            {
                gameObject.SetActive(false);
                return;
            }

            Image image = GetComponent<Image>();

            if (image != null)
            {
                image.sprite = convertedSprite;
            }

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = convertedSprite;
            }
        }
    }

}
