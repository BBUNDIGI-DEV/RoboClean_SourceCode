using RoboClean.Data;
using RoboClean.Input;
using RoboClean.Player;
using RoboClean.Stage;
using UnityEngine;

namespace RoboClean.Character.Player
{
    public class PlayerInteractiveManager : MonoBehaviour
    {
        private Rigidbody RB;
        private RuntimePlayerInfo mPlayerInfo
        {
            get
            {
                return RuntimeDataLoader.PlayerRuntimeInfo;
            }
        }

        private InteractableBase mDectedObjectOrNull;

        private void Awake()
        {
            RB = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            InputManager.Instance.AddInputCallback(eInputSections.BattleGamePlay, eBattleInputName.Interactive.ToString(), tryInteractive);
        }

        public void SetDetectedObject(InteractableBase baseObjectOrNull)
        {
            if (baseObjectOrNull == null)
            {
                mDectedObjectOrNull = null;
                mPlayerInfo.DetectedInteractableObject.Value = eInteractableType.None;
                return;
            }

            mDectedObjectOrNull = baseObjectOrNull;
            mPlayerInfo.DetectedInteractableObject.Value = baseObjectOrNull.ObjectType;
        }

        private void tryInteractive(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (mDectedObjectOrNull == null)
            {
                return;
            }
            mPlayerInfo.DetectedInteractableObject.Value = eInteractableType.None;
            mDectedObjectOrNull.InvokeInteraction();
            mDectedObjectOrNull = null;
        }
    }
}
