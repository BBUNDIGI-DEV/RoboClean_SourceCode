using RoboClean.Managers;
using RoboClean.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RoboClean.Input
{
    public abstract class InputHandlerBase : MonoBehaviour
    {
        public ObservedData<eInputDeviceType> CurrentInputDevice;
        protected eInputSections mCurrentActivateMap;

        public abstract void SetEnabled(bool enabled);
        public abstract Vector3 GetMoveDir();
        public abstract Vector3 GetAttackAim(Rigidbody playerRB);
        public abstract void SwitchActionMap(eInputSections newMap);
        public abstract void AddInputCallback
            (eInputSections map, string action, System.Action<InputAction.CallbackContext> callback);
        public abstract void RemoveInputCallback
            (eInputSections map, string action, System.Action<InputAction.CallbackContext> callback);
        public abstract void DisableHandler();
        public abstract bool CheckPlatform(RuntimePlatform platform);

        protected bool checkPlatform(RuntimePlatform platform, params RuntimePlatform[] includedPlatforms)
        {
            int usingPlatform = 0;
            for (int i = 0; i < includedPlatforms.Length; i++)
            {
                usingPlatform |= 1 << (int)includedPlatforms[i];
            }
            return (usingPlatform & (1 << (int)platform)) == (1 << (int)platform);
        }

        protected virtual void Awake()
        {
            if (SceneSwitchingManager.Instance.CurrentScene == eSceneName.scene_title)
            {
                SwitchActionMap(eInputSections.UI);
            }
            else if (SceneSwitchingManager.Instance.CurrentScene == eSceneName.scene_story_cutscene)
            {
                SwitchActionMap(eInputSections.CutScene);
            }
            else
            {
                SwitchActionMap(eInputSections.BattleGamePlay);
            }
        }
    }
}