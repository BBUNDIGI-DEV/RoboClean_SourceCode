using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using RoboClean.Utils;

namespace RoboClean.Input
{
    public class PCInputHandler : InputHandlerBase
    {
        public class ActionMap
        {
            public InputAction this[string Tag]
            {
                get
                {
                    return mActionDic[Tag];
                }
            }

            private Dictionary<string, InputAction> mActionDic;

            public void InitializeActionDic<T>(InputActionAsset actionAsset, eInputSections actionMapName) where T : System.Enum
            {
                mActionDic = new Dictionary<string, InputAction>();
                InputActionMap actionMap = actionAsset.FindActionMap(actionMapName.ToString());
                Debug.Assert(actionMap != null, $"Cannot found action map: [{actionMapName}]");
                string[] enumString = System.Enum.GetNames(typeof(T));
                for (int i = 0; i < enumString.Length; i++)
                {
                    var action = actionMap.FindAction(enumString[i]);
                    Debug.Assert(actionMap != null, $"Cannot found action in action map" +
                        $": [Action Map {actionMapName}, Action {enumString[i]}]");
                    mActionDic.Add(enumString[i], action);
                }
            }
        }

        [SerializeField] private InputActionAsset INPUT_ACTIONS;
        private Dictionary<eInputSections, ActionMap> mActionMaps;

        protected override void Awake()
        {
            mActionMaps = new Dictionary<eInputSections, ActionMap>();
            InputSystem.onEvent += onAnyInput;
            for (int i = 0; i < (int)eInputSections.Count; i++)
            {
                eInputSections curMap = (eInputSections)i;
                ActionMap actionMap = new ActionMap();
                switch (curMap)
                {
                    case eInputSections.BattleGamePlay:
                        actionMap.InitializeActionDic<eBattleInputName>(INPUT_ACTIONS, curMap);
                        break;
                    case eInputSections.CutScene:
                        actionMap.InitializeActionDic<eCutSceneInputName>(INPUT_ACTIONS, curMap);
                        break;
                    case eInputSections.Dialouge:
                        actionMap.InitializeActionDic<eDialougeInputName>(INPUT_ACTIONS, curMap);
                        break;
                    case eInputSections.Menu:
                        actionMap.InitializeActionDic<eMenuInputName>(INPUT_ACTIONS, curMap);
                        break;
                    case eInputSections.UI:
                        actionMap.InitializeActionDic<eUIInputName>(INPUT_ACTIONS, curMap);
                        break;
                    default:
                        Debug.LogError($"Cannot use this enum value [{curMap}]");
                        break;
                }
                INPUT_ACTIONS.FindActionMap(curMap.ToString()).Disable();
                mActionMaps.Add(curMap, actionMap);
            }
        }

        private void OnDestroy()
        {
            InputSystem.onEvent -= onAnyInput;
        }


        public override Vector3 GetMoveDir()
        {
            var moveAction = mActionMaps[eInputSections.BattleGamePlay][eBattleInputName.Move.ToString()];
            if (moveAction.activeControl != null)
            {

            }
            Vector2 moveInputValue = moveAction.ReadValue<Vector2>();
            Vector3 moveDir = new Vector3(moveInputValue.x, 0.0f, moveInputValue.y);
            moveDir = moveDir.normalized;
            return moveDir;
        }

        public override Vector3 GetAttackAim(Rigidbody playerRB)
        {
            if (CurrentInputDevice == eInputDeviceType.GamePad)
            {
                Vector3 moveDir = GetMoveDir();
                if (moveDir != Vector3.zero)
                {
                    return moveDir;
                }
            }
            else
            {
                Vector3 playerPos = playerRB.position;
                Plane charPlane = new Plane(Vector3.up, playerPos);
                Vector3 planeHitPoint;
                Vector3 playerToMouse = Vector3.zero;
                Vector3 returnDir = Vector3.forward;
                Vector2 currentMousePos = Mouse.current.position.ReadValue();
                if (CameraUtils.Instance.ScreenPointToPlaneHitPoint(currentMousePos, charPlane, out planeHitPoint))
                {
                    playerToMouse = planeHitPoint - playerPos;
                    playerToMouse.Normalize();
                    returnDir = playerToMouse;
                    return returnDir;
                }
            }
            return playerRB.GetForward();
        }

        public override void SwitchActionMap(eInputSections newMap)
        {
            if (mCurrentActivateMap == newMap)
            {
                return;
            }

            if (mCurrentActivateMap != eInputSections.None)
            {
                INPUT_ACTIONS.FindActionMap(mCurrentActivateMap.ToString()).Disable();
            }
            INPUT_ACTIONS.FindActionMap(newMap.ToString()).Enable();
            mCurrentActivateMap = newMap;
        }

        public override void AddInputCallback(eInputSections map, string action, System.Action<InputAction.CallbackContext> callback)
        {
            mActionMaps[map][action].started += callback;
        }

        public override void RemoveInputCallback(eInputSections map, string action, System.Action<InputAction.CallbackContext> callback)
        {
            mActionMaps[map][action].started -= callback;
        }

        public override void DisableHandler()
        {
            gameObject.SetActive(false);
        }

        public override bool CheckPlatform(RuntimePlatform platform)
        {
            return checkPlatform(platform, RuntimePlatform.WindowsEditor, RuntimePlatform.WindowsPlayer);
        }

        public override void SetEnabled(bool enabled)
        {
            if (enabled)
            {
                INPUT_ACTIONS.FindActionMap(mCurrentActivateMap.ToString()).Enable();
            }
            else
            {
                INPUT_ACTIONS.FindActionMap(mCurrentActivateMap.ToString()).Disable();
            }
        }

        private void onAnyInput(UnityEngine.InputSystem.LowLevel.InputEventPtr eventPtr, InputDevice device)
        {
            if (!eventPtr.IsA<UnityEngine.InputSystem.LowLevel.StateEvent>()
                && !eventPtr.IsA<UnityEngine.InputSystem.LowLevel.DeltaStateEvent>())
            {
                return;
            }

            bool isNoisyEvent = true;
            foreach (var control in eventPtr.EnumerateChangedControls())
            {
                if (control.device is not Gamepad)
                {
                    isNoisyEvent = false;
                    break;
                }

                if (control.shortDisplayName == null)
                {
                    isNoisyEvent = false;
                    return;
                }

                if (!control.shortDisplayName.Contains("LS") && !control.shortDisplayName.Contains("RS"))
                {
                    isNoisyEvent = false;
                    break;
                }

                float stickDelta = (float)control.ReadValueFromEventAsObject(eventPtr);
                if (stickDelta > 0.05f)
                {
                    isNoisyEvent = false;
                    break;
                }
            }

            if (isNoisyEvent)
            {
                return;
            }

            eInputDeviceType deviceType = default;

            if (device is Gamepad)
            {
                deviceType = eInputDeviceType.GamePad;
            }
            if (device is Keyboard || device is Mouse)
            {
                deviceType = eInputDeviceType.KeyboardAndMouse;
            }

            if (CurrentInputDevice != deviceType)
            {
                CurrentInputDevice.Value = deviceType;
                Debug.Log($"Received event for {device}");
            }
        }
    }
}