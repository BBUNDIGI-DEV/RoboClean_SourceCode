using UnityEngine;
using TMPro;
using RoboClean.Utils;
using RoboClean.Input;

namespace RoboClean.UI
{
    public class InputDeviceTextSwapHelper : DestoryOnlyIfAwakeMonoBehavior
    {
        [SerializeField] private string KEYBAORD_TEXT;
        [SerializeField] private string GAMEPAD_TEXT;
        [SerializeField] private string MOBILE_TEXT;

        private void Start()
        {
            InputManager.Instance.LoadedInputHandler.CurrentInputDevice.AddListener(textSwap, true);
        }

        protected override void onDisable()
        {
        }

        protected override void onDestory()
        {
            if (!InputManager.IsExist)
            {
                return;
            }

            InputManager.Instance.LoadedInputHandler.CurrentInputDevice.RemoveListener(textSwap);
        }

        public void textSwap(eInputDeviceType inputDevice)
        {
            string convertedString = "";
            Debug.Log(inputDevice);
            switch (inputDevice)
            {
                case eInputDeviceType.KeyboardAndMouse:
                    convertedString = KEYBAORD_TEXT;
                    break;
                case eInputDeviceType.GamePad:
                    convertedString = GAMEPAD_TEXT;
                    break;
                case eInputDeviceType.Mobile:
                    convertedString = MOBILE_TEXT;
                    break;
                default:
                    break;
            }
            TMP_Text textComponent = GetComponent<TMP_Text>();
            textComponent.text = convertedString;
        }
    }

}
