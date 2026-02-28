using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RoboClean.Utils;
using RoboClean.Input;

namespace RoboClean.UI
{
    public class NavigationSetter : DestoryOnlyIfAwakeMonoBehavior
    {
        private static int smSetterPriority = -1;
        [SerializeField] private Button UI_FIRST_SELECTED;
        [SerializeField] private int SETTER_PRIORITY;
        private Button[] mButtonInChild;

        private void Start()
        {
            InputManager.Instance.LoadedInputHandler.CurrentInputDevice.AddListener(TryUpDateUINav);
        }

        private void Update()
        {
            if (SETTER_PRIORITY > smSetterPriority)
            {
                smSetterPriority = SETTER_PRIORITY;
                TryUpDateUINav();
            }
        }

        private void TryUpDateUINav()
        {
            if (mButtonInChild == null)
            {
                mButtonInChild = GetComponentsInChildren<Button>();
            }

            if (InputManager.Instance.LoadedInputHandler.CurrentInputDevice == eInputDeviceType.GamePad)
            {
                UI_FIRST_SELECTED.Select();
                for (int i = 0; i < mButtonInChild.Length; i++)
                {
                    mButtonInChild[i].navigation = Navigation.defaultNavigation;
                }
            }
            else
            {
                Navigation noneNav = new Navigation();
                noneNav.mode = Navigation.Mode.None;
                for (int i = 0; i < mButtonInChild.Length; i++)
                {
                    mButtonInChild[i].navigation = noneNav;
                }
            }
        }

        protected override void onDisable()
        {
            if (EventSystem.current != null) //In Case EventSystem is destoryd first when scene is unloaded
            {
                EventSystem.current.firstSelectedGameObject = null;
            }
            smSetterPriority = -1;
        }

        protected override void onDestory()
        {
            if (!InputManager.IsExist)
            {
                return;
            }

            InputManager.Instance.LoadedInputHandler.CurrentInputDevice.RemoveListener(TryUpDateUINav);
        }
    }

}
