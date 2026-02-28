using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RoboClean.Input
{
    public class MobileButton : MonoBehaviour, IPointerEnterHandler
    {
        public string Usage
        {
            get
            {
                return SD_USAGE;
            }
        }

        private Action<InputAction.CallbackContext> mCallback;
        [SerializeField] private string SD_USAGE;

        public void AddInputCallback(Action<InputAction.CallbackContext> callback)
        {
            mCallback += callback;
        }

        public void RemoveInputCallback(Action<InputAction.CallbackContext> callback)
        {
            mCallback -= callback;
        }

        public void ClearCallback()
        {
            mCallback = null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            mCallback?.Invoke(default(InputAction.CallbackContext));
        }
    }
}


